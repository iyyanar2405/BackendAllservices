using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AuthProvider.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.FileProviders;
using AuthProvider.Context;
using AuthProvider.Settings;
using Duende.IdentityServer.EntityFramework.Stores;
using CoreIdentity.API.Settings;
using System.Security.Cryptography.X509Certificates;
using Twilio.TwiML.Voice;
using Hadvida.Shared.Managers;
using AuthProvider.Swagger;

namespace Hadvida.AuthProvider
{
    public class Startup
    {
        //2147483646 siteId represents the SSRS Service user
        const int SSRSUser = 2147483646;
        const string CertPass = "password123";

        public Startup(IConfiguration configuration)
        {
            //put the environment as a field in the appsettings so it was tied to the publish
            string envName = configuration.GetValue<string>("Environment_Name");
            //  Configuration = configuration;
            
              Configuration = (IConfiguration)new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();


            AppSettings.SetAuthProviderConnectionString(Configuration.GetConnectionString("AuthProviderConn"));

            AppSettings.CertificateName = Configuration.GetValue<string>("Certificate");
            //if  (!string.IsNullOrWhiteSpace(AppSettings.CertificateName))
            //{
            //    try
            //    {
            //        X509Certificate2 certificate = CertificateManager.GetCertificate(AppSettings.CertificateName);
            //        var aesManager = new AESManager(certificate);
            //        AppSettings.SetAuthProviderConnectionString(AppSettings.GetAuthProviderConnectionString());
            //    }
            //    catch
            //    {
            //        // throw new Exception("Failed to decrypt connection string"); 
            //    }
                
                
            //}

            //AppSettings.SetConfig(Configuration);
            AppSettings.HadvidaEmailServiceUrl = Configuration.GetValue<string>("EmailServiceUrl");
            AppSettings.SetChorusApiBaseUrl(Configuration.GetValue<string>("ChorusAppApi"));
            AppSettings.AllowEmails = Configuration.GetValue<bool>("Allow_Emails");
            AppSettings.RefreshTokenExpireTime = Configuration.GetValue<int>("RefreshTokenExpireTime");
            AppSettings.TfaTokenExpireTime = Configuration.GetValue<int>("TfaTokenExpireTime");
            AppSettings.JwtExpireTime = Configuration.GetValue<int>("JwtExpireTime");
            AppSettings.SessionExpireTime = Configuration.GetValue<int>("SessionExpireTime");
            AppSettings.MaxFailedAccessAttempts = Configuration.GetValue<int>("MaxFailedAccessAttempts");
        }

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ClientAppSettings>(Configuration.GetSection("ClientAppSettings"));

            services.AddDbContext<AuthProviderContext>(options =>
                options.UseSqlServer(AppSettings.GetAuthProviderConnectionString()));

            // Add Customer Portal Context
            services.AddDbContext<CustomerPortalContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("CustomerPortalConn")));

            services.AddTransient<AuthProviderKeyContext>();
            IServiceProvider authProviderService = services.BuildServiceProvider();

            //Set All of our Identity Options for signin, login, lockout, etc....
            services.Configure<IdentityOptions>(options =>
            {
                //SignIn
                options.SignIn.RequireConfirmedAccount = true;

                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = AppSettings.MaxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = false;

                // User settings
                options.User.RequireUniqueEmail = true;
                options.Tokens.AuthenticatorIssuer = "Hadvida Inc";
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<AuthProviderContext>()
               .AddSignInManager()
               .AddUserManager<UserManager<ApplicationUser>>()
               .AddDefaultTokenProviders();


            // Get an instance of the service from the service provider
            var authKeyService = authProviderService.GetService<AuthProviderKeyContext>();
            if (AppSettings.GetJwtKey() == null)
            {
                try
                {
                    using (var context = authKeyService)
                    {
                        var certStr = context.getKey.Where(w => w.Site_ID == int.Parse(AppSettings.AuthProviderKeyId)).First().Password;
                        var certBytes = Convert.FromBase64String(certStr);
                        //2147483645 siteId represent the AuthProvider Signature Key so that all api's can decrypt the JWT with same key
                        AppSettings.SetJwtKey(certBytes);
                    }
                }
                catch (Exception ex)
                {
                    // During EF migrations or initial setup, the database may not be fully initialized
                    // Log and continue - the JWT key will be loaded when the application runs properly
                    Console.WriteLine($"Warning: Could not load JWT key from database during startup: {ex.Message}");
                }
            }

            //X509Certificate2 certificate = new X509Certificate2(AppSettings.GetJwtKey(), CertPass);

            services.AddIdentityServer(options => { 
                    options.KeyManagement.Enabled = false; 
                 })
                .AddApiAuthorization<ApplicationUser, AuthProviderContext>()
                //.AddSigningCredential(certificate)
                .AddDeviceFlowStore<DeviceFlowStore>();
            
            //Adds the JWT Authentication and Validation
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                   // IssuerSigningKey = new X509SecurityKey(certificate),
                    ValidateIssuer = true,
                    RoleClaimType = "Role",
                    NameClaimType = "Claim",
                    ValidIssuer = "Hadvida Inc",
                    ValidateAudience = false
                };
            });


            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.AllowAnyHeader();
                                      builder.AllowAnyOrigin();
                                      builder.AllowAnyMethod();
                                  });
            });

            services.AddRazorPages();
            services.AddControllers();


            services.AddSwaggerGen(o => { 
                o.AddSecurityDefinition("jwt_auth", new OpenApiSecurityScheme
                {
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                o.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "jwt_auth"
                            },
                            Scheme = "oauth2",
                            Name = "Authorization",
                            In = ParameterLocation.Header
                        },
                        new string[]{ }
                    }
                });

                // Fix schema ID conflicts by using full type name
                o.CustomSchemaIds(type => 
                {
                    var typeName = type.FullName?.Replace("+", ".");
                    if (typeName?.Contains("CustomerPortal") == true)
                    {
                        return typeName.Replace("AuthProvider.DTOs.CustomerPortal.", "CustomerPortal_")
                                      .Replace("AuthProvider.Models.CustomerPortal.", "CustomerPortal_")
                                      .Replace("`1[", "_Of_")
                                      .Replace("`2[", "_Of_")
                                      .Replace("]", "")
                                      .Replace(",", "_")
                                      .Replace(" ", "");
                    }
                    return typeName?.Replace("AuthProvider.DTOs.", "")
                                    .Replace("AuthProvider.Models.", "")
                                    .Replace("`1[", "_Of_")
                                    .Replace("`2[", "_Of_")
                                    .Replace("]", "")
                                    .Replace(",", "_")
                                    .Replace(" ", "");
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    o.IncludeXmlComments(xmlPath);
                }

                o.SwaggerGeneratorOptions.Servers.Add(new OpenApiServer { Url = "../../" });
                
                // Enhanced API documentation with Customer Portal integration
                o.SwaggerDoc("v1", new OpenApiInfo { 
                    Title = "AuthProvider & Customer Portal API", 
                    Version = "v1",
                    Description = @"
## Comprehensive Authentication and Customer Portal Management API

This API provides two main areas of functionality:

### ?? **Authentication & Authorization (AuthProvider)**
- User authentication and JWT token management
- Identity server integration with OAuth2/OpenID Connect
- User account management and security features

### ?? **Customer Portal Management**
Complete CRUD operations for Customer Portal database with **27 entities**:

#### **Core Business Entities**
- **Actions** - Action items and task management
- **Audits** - Comprehensive audit lifecycle management  
- **AuditTypes** - Audit type categorization
- **AuditServices** - Services associated with audits
- **AuditTeamMembers** - Audit team management

#### **Location & Organization**
- **Countries** - Country master data with full hierarchy
- **Cities** - City management with country relationships
- **Companies** - Company information and management
- **Sites** - Company site/location management
- **Services** - Service catalog management

#### **Content Management**
- **Chapters** - Document chapter organization
- **Clauses** - Document clauses within chapters

#### **User Management**
- **Users** - Customer portal user accounts
- **Roles** - Role-based access control
- **UserRoles** - User role assignments

#### **Access Control**
- **UserCityAccess** - City-level user permissions
- **UserCountryAccess** - Country-level user permissions  
- **UserServiceAccess** - Service-level user permissions
- **UserNotificationAccess** - Notification preferences

#### **Training & Development**
- **Trainings** - Training program management
- **UserTrainings** - User training enrollments and progress

#### **Configuration & Categories**
- **FindingCategories** - Audit finding categorization
- **FindingStatuses** - Finding status management
- **FocusAreas** - Audit focus area definitions
- **NotificationCategories** - Notification categorization

#### **System & Preferences**
- **ErrorLogs** - System error tracking and logging
- **UserPreferences** - Individual user preference settings

### ? **Key Features**
- **Database First Approach** - Works with existing Customer_Portal database
- **Comprehensive Pagination** - All list endpoints support 1-100 items per page
- **Advanced Search & Filtering** - Multi-criteria search capabilities
- **Status Management** - Activate/deactivate entities across the system
- **Relationship Navigation** - Access related entities (e.g., cities by country)
- **JWT Authentication** - Secure access to all Customer Portal endpoints
- **Validation & Error Handling** - Comprehensive input validation with detailed error messages
- **Standardized API Responses** - Consistent response format across all endpoints

### ?? **API Endpoints Structure**
Each entity follows a consistent RESTful pattern:
- `GET /api/customerportal/{entity}` - Paginated list with filtering
- `GET /api/customerportal/{entity}/{id}` - Get specific item
- `POST /api/customerportal/{entity}` - Create new item  
- `PUT /api/customerportal/{entity}/{id}` - Update existing item
- `DELETE /api/customerportal/{entity}/{id}` - Delete item
- `PATCH /api/customerportal/{entity}/{id}/status` - Status management
- `GET /api/customerportal/{entity}/search` - Search functionality

### ?? **Database Integration**
- **Customer_Portal Database** - Complete integration with existing database
- **27 Tables** - Full CRUD operations for all entities
- **Relationship Preservation** - All foreign keys and relationships maintained
- **Schema Compliance** - Matches existing database constraints and structure

### ?? **Security**
- **JWT Bearer Authentication** required for all Customer Portal endpoints
- **Role-based Authorization** support
- **Input Validation** with comprehensive error reporting
- **SQL Injection Protection** through Entity Framework Core

### ?? **Performance**
- **Efficient Querying** with Entity Framework Core
- **Pagination** to handle large datasets
- **Selective Loading** of related data
- **Optimized Database Queries** with proper indexing support

For detailed endpoint documentation, expand the sections below or refer to the individual controller documentation.
",
                    Contact = new OpenApiContact
                    {
                        Name = "Hadvida Inc - Customer Portal Team",
                        Email = "support@hadvida.com",
                        Url = new Uri("https://hadvida.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    }
                });
                
                // Configure Swagger UI to show examples and organize by tags
                o.UseInlineDefinitionsForEnums();
                o.OrderActionsBy(apiDesc => apiDesc.GroupName);
                
                // Add custom schema filters for better documentation
                o.SchemaFilter<SwaggerSchemaFilter>();
                o.OperationFilter<SwaggerOperationFilter>();
                
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            string envName = Configuration.GetValue<string>("Environment_Name");
            AppSettings.Setenv(envName);
            if (Configuration.GetValue<bool>("IsDbMigration"))
                AppSettings.SetJwtKey(Encoding.UTF8.GetBytes("IsDbMigration"));
            else
            {
                var service = app.ApplicationServices.GetServices<AuthProviderKeyContext>();
                if (AppSettings.GetJwtKey() == null || AppSettings.GetJwtKey().Length <= 0)
                {
                    try
                    {
                        using (var context = service.FirstOrDefault())
                        {
                            if (context != null)
                            {
                                var key = context.getKey?.FirstOrDefault(w => w.Site_ID == int.Parse(AppSettings.AuthProviderKeyId));
                                if (key != null)
                                {
                                    var certBytes = Convert.FromBase64String(key.Password);
                                    //2147483645 siteId represent the AuthProvider Signature Key so that all api's can decrypt the JWT with same key
                                    AppSettings.SetJwtKey(certBytes);
                                }
                                else
                                {
                                    Console.WriteLine("Warning: JWT key not found in database. Using development key.");
                                    // Use a minimal development key if the database key is not available
                                    AppSettings.SetJwtKey(Encoding.UTF8.GetBytes("development-key-placeholder"));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not load JWT key from database during Configure: {ex.Message}");
                        // Use a minimal development key if the database is not accessible
                        AppSettings.SetJwtKey(Encoding.UTF8.GetBytes("development-key-placeholder"));
                    }
                }
            }


            if (!string.Equals("prod", envName, StringComparison.OrdinalIgnoreCase))
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
              //  app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();
                

            app.UseSwaggerUI(s =>
            {
                // Use dynamic Swagger generation instead of static file
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthProvider & Customer Portal API v1");
                s.RoutePrefix = "swagger"; // This makes Swagger available at /swagger
                
                // Enhanced Swagger UI configuration
                s.DisplayRequestDuration();
                s.EnableDeepLinking();
                s.EnableFilter();
                s.ShowExtensions();
                s.EnableValidator();
                
                // Group endpoints by tags for better organization
                s.DefaultModelsExpandDepth(2);
                s.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
                s.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                
                // Custom CSS for better appearance
                s.InjectStylesheet("/files/swagger-custom.css");
            });            
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors(MyAllowSpecificOrigins);
            app.UseRouting();
            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath,"swagger")),
                RequestPath = "/files"
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}