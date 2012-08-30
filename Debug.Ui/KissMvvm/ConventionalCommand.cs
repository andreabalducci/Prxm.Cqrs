using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Diagnostics;
using Fasterflect;
using System.Windows.Input;
using Sample.DebugUi.KissMvvm;

namespace Sample.DebugUi.KissMvvm
{
    /// <summary>
    /// Questo comando serve per fare binding per convenzione, ovvero nel ViewModel invece di creare il
    /// DelegatEcommand, specificare tutto quanto etc etc basta fare una funzione
    /// ExecuteXXX
    /// CanExecuteXXX
    /// 
    /// e quindi bindare con una sintassi tipo
    ///  Command="{mvvmCommands:MvvmCommand Path=XXX}"
    ///  Tutto il resto lo fa lui.
    /// </summary>
    public class ConventionalCommand : ICommand
    {
        private FrameworkElement boundObject;

        private String path;

        private Boolean isAsync;

        private String waitMessage;

        public ConventionalCommand(FrameworkElement boundObject, String path, Boolean isAsync, String waitMessage)
        {
            this.boundObject = boundObject;
            this.path = path;
            this.isAsync = isAsync;
            this.waitMessage = waitMessage;
            this.boundObject.DataContextChanged += boundObject_DataContextChanged;
        }

        void boundObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RelatedViewModel = e.NewValue as BaseViewModel;
        }

        BaseViewModel relatedViewModel;
        MethodInfo canExecute;
        MethodInfo execute;

        private BaseViewModel RelatedViewModel
        {

            get
            {
                return relatedViewModel;
            }
            set
            {
                if (relatedViewModel != null)
                {
                    relatedViewModel.PropertyChanged -= relatedViewModel_PropertyChanged;
                }
                relatedViewModel = value;
                if (relatedViewModel != null)
                {
                    Type vmt = relatedViewModel.GetType();
                    canExecute = vmt.GetMethod("CanExecute" + path, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    execute = vmt.GetMethod("Execute" + path, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (execute == null)
                    {
                        Debug.WriteLine("Binding Error - Command Execute" + path + " not found in viewmodel of type " + vmt.GetType());
                    }
                    relatedViewModel.PropertyChanged += relatedViewModel_PropertyChanged;
                }
                OnCanExecuteChanged();
            }
        }

        void relatedViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanExecute" + path)
            {
                OnCanExecuteChanged();
            }
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler temp = CanExecuteChanged;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }

        public bool CanExecute(object parameter)
        {
            if (RelatedViewModel == null) return false;
            if (canExecute == null) return true;

            return (Boolean)canExecute.Call(RelatedViewModel, new Object[] { parameter });
        }

        public void Execute(object parameter)
        {
            if (RelatedViewModel == null) return;

            if (!isAsync)
            {
                execute.Call(RelatedViewModel, new Object[] { parameter });
            }
            else
            {
                RelatedViewModel.Execute(() => execute.Call(RelatedViewModel, new Object[] { parameter }), true, waitMessage);
            }

        }
    }

    public class MvvmCommand : MarkupExtension
    {

        /// <summary>
        /// This is the path of the command, the ViewModel should have a method called ExecutePath to 
        /// make everything work.
        /// </summary>
        public String Path { get; set; }

        [DefaultValue(false)]
        public Boolean IsAsync { get; set; }

        [DefaultValue("")]
        public String WaitMessage { get; set; }

        public MvvmCommand()
        {
            WaitMessage = String.Empty;
            Path = String.Empty;
        }

        public MvvmCommand(String path)
            : this()
        {
            this.Path = path;
        }

        /// <summary>
        /// we need to provide the ICommand that will take care of command invocation.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service == null) return false;

            FrameworkElement dobj = service.TargetObject as FrameworkElement;
            if (dobj == null) throw new ApplicationException("Cannot do Conventional Command Binding");
            return new ConventionalCommand(dobj, Path, IsAsync, WaitMessage);
        }
    }
}
