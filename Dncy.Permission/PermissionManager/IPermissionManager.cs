//using System.Diagnostics.CodeAnalysis;
//using System.Threading.Tasks;
//using Dncy.Permission.Models;

//namespace Dncy.Permission
//{
//    public interface IPermissionManager
//    {
//        Task<bool> IsGrantedAsync([NotNull] string name, [MaybeNull] string providerName, [MaybeNull] string providerKey);

//        Task<MultiplePermissionGrantResult> IsGrantedAsync([NotNull] string[] names, [MaybeNull] string providerName, [MaybeNull] string providerKey);
//    }
//}