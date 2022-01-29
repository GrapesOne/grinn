using Delivery.Prefabs;
using Gateway;
using Interactor;
using Zenject;

namespace Installer
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AccountInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AccountGateway>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ChangeViewInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ShopInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ShopGateway>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DownloadGateway>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<DownloadInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<NotificationInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BasketInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ShopLocationInteractor>().AsSingle().NonLazy();
			Container.BindInterfacesAndSelfTo<OrderInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<FavouritesInteractor>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<LoadingInteractor>().AsSingle().NonLazy();

        }
    }
}
