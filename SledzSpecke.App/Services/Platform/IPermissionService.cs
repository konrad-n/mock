// Services/Platform/PermissionService.cs
public interface IPermissionService
{
    Task<PermissionStatus> CheckPermissionAsync<TPermission>()
        where TPermission : Permissions.BasePermission, new();

    Task<PermissionStatus> RequestPermissionAsync<TPermission>()
        where TPermission : Permissions.BasePermission, new();

    Task<IDictionary<Type, PermissionStatus>> RequestPermissionsAsync(params Type[] permissionTypes);

    Task<bool> HandleDeniedPermissionAsync<TPermission>(string rationaleMessage)
        where TPermission : Permissions.BasePermission, new();
}