﻿using Prism.Navigation;

namespace Prism.Segue.Application.ViewModels
{
    public class SecretPageModel : ViewModelBase
    {
        private bool _isHierarchical;
        private bool _isModal;

        public SecretPageModel(INavigationService navigationService) : base(navigationService)
        {
        }

        public bool IsHierarchical
        {
            get => _isHierarchical;
            set => SetProperty(ref _isHierarchical, value);
        }

        public bool IsModal
        {
            get => _isModal;
            set => SetProperty(ref _isModal, value);
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            if (parameters.TryGetValue("isModal", out bool isModal))
            {
                IsModal = isModal;
                IsHierarchical = !isModal;
            }
            else
            {
                IsModal = false;
                IsHierarchical = true;
            }
        }
    }
}