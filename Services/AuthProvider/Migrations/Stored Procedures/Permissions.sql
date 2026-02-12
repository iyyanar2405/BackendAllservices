USE [AuthProvider];
GO

GRANT EXECUTE ON [dbo].[AddTokenRequest] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[DataDecrypt] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[DataEncrypt] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[Permissions] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[TokenDecrypt] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[TokenEncrypt] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[TokenValid] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[TouchToken] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[_AddTokenRequest] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[_DataDecrypt] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[_DataEncrypt] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[_TokenDecrypt] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[_TokenEncrypt] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[_TokenValid] TO [db_execute_sp]
GRANT EXECUTE ON [dbo].[_TouchToken] TO [db_execute_sp]