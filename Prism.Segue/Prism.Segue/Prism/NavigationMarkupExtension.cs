﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Prism.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.Segue.Application.Prism
{
    public abstract class NavigationMarkupExtension : IMarkupExtension, ICommand
    {
        protected INavigationService NavigationService;
        private IRootObjectProvider _rootObjectProvider;
        private IProvideValueTarget _valueTargetProvider;
        protected const string NavParameterMessage = "Command Parameter must be of type NavigationParameter";
        protected void InitNavService()
        {
            if (NavigationService != null) return;
            // if XamlCompilation is active, IRootObjectProvider is not available, but SimpleValueTargetProvider is available
            // if XamlCompilation is inactive, IRootObjectProvider is available, but SimpleValueTargetProvider is not available
            object rootObject;
            //object bindable;
            if (_rootObjectProvider == null && _valueTargetProvider == null)
                throw new ArgumentException("serviceProvider does not provide an IRootObjectProvider or SimpleValueTargetProvider");
            if (_rootObjectProvider == null)
            {
                PropertyInfo propertyInfo = _valueTargetProvider.GetType().GetTypeInfo().DeclaredProperties.FirstOrDefault(dp => dp.Name.Contains("ParentObjects"));
                if (propertyInfo == null) throw new ArgumentNullException("ParentObjects");

                var parentObjects = (propertyInfo.GetValue(_valueTargetProvider) as IEnumerable<object>).ToList();
                var parentObject = parentObjects.FirstOrDefault(pO => pO.GetType().GetTypeInfo().IsSubclassOf(typeof(Page)));

                //bindable = parentObjects.FirstOrDefault();
                rootObject = parentObject ?? throw new ArgumentNullException("parentObject");
            }
            else
            {
                rootObject = _rootObjectProvider.RootObject;
                // bindable = _valueTargetProvider.TargetObject;
            }

            if (rootObject is Page page && page.BindingContext is INavigatable vm)
                NavigationService = vm.NavigationService;
            //if (segueItem != null && segueItem is BindableObject bindable) 
            //    _navigationParameters = GetNavigationParameters(bindable);
        }

        protected void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");
            _valueTargetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            _rootObjectProvider = serviceProvider.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider;
            return this;
        }
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
        public event EventHandler CanExecuteChanged;
    }
}