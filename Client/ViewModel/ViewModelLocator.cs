using Client.Infra;
using Client.Services;
using CommonServiceLocator;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using System;

namespace Client.ViewModel
{
   
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LobbyViewModel>();
            SimpleIoc.Default.Register<SignInViewModel>();
            SimpleIoc.Default.Register<RegisterViewModel>();
            SimpleIoc.Default.Register<GameViewModel>();
          
                                 
            SimpleIoc.Default.Register<IChatClientManager,ChatClientManager>();
            SimpleIoc.Default.Register<Infra.IDialogService, DialogService>();
            SimpleIoc.Default.Register<INavigationService, NavigationService>();


            var navigationService = new FrameNavigationService();
            navigationService.Configure("SignInView", new Uri("../Views/SignInView.xaml", UriKind.Relative));
            navigationService.Configure("LobbyView", new Uri("../Views/LobbyView.xaml", UriKind.Relative));
            navigationService.Configure("RegisterView", new Uri("../Views/RegisterView.xaml", UriKind.Relative));

            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }

        public MainViewModel MainVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public SignInViewModel SignInVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SignInViewModel>();
            }
        }

        public RegisterViewModel RegisterVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<RegisterViewModel>();
            }
        }

        public LobbyViewModel LobbyVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LobbyViewModel>();
            }
        }

        public GameViewModel GameVM
        {
            get
            {
                return ServiceLocator.Current.GetInstance<GameViewModel>(Guid.NewGuid().ToString());
            }
        }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}