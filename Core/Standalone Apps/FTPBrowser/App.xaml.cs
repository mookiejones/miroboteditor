﻿using System;
using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using FTPBrowser.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace FTPBrowser
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
           /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        public App()
        {
            Services = ConfigureServices();
            Ioc.Default.ConfigureServices(Services);
        }


        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Services
 
//            services.AddSingleton<ICommandService,CommandService>();
            // Viewmodels

            services.AddTransient<FTPAccountViewModel>();



            return services.BuildServiceProvider();
        }
    }
}
