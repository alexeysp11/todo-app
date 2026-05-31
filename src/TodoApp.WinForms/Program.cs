using System;
using System.IO;
using System.Windows.Forms;
using Autofac;
using AutofacSerilogIntegration;
using Serilog;
using TodoApp.WinForms.DataAccess.Abstractions;
using TodoApp.WinForms.DataAccess.Repositories;
using TodoApp.WinForms.DataAccess.Repositories.Implementations;
using TodoApp.WinForms.Services.Abstractions;
using TodoApp.WinForms.Services.Implementations;
using TodoApp.WinForms.Views;

namespace TodoApp.WinForms
{
    static class Program
    {
        public static IContainer Container { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "todo_log-.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Globally trap errors in the UI thread (so that they go to Serilog, not to the standard Windows window)
            Application.ThreadException += (sender, e) =>
            {
                Log.Fatal(e.Exception, "Unhandled exception on WinForms UI thread.");
                MessageBox.Show("A critical error has occurred. The application will now close. Details in the log.",
                                "Critical error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            };

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Global error handling in background threads (Tasks, thread pool)
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Log.Fatal(e.ExceptionObject as Exception, "A critical unhandled exception occurred in the application domain.");
            };

            var builder = new ContainerBuilder();

            builder.RegisterLogger();

            builder.RegisterType<TaskRepository>().As<ITaskRepository>().SingleInstance();
            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>().SingleInstance();
            builder.RegisterType<PriorityRepository>().As<IPriorityRepository>().SingleInstance();
            builder.RegisterType<TaskStatusRepository>().As<ITaskStatusRepository>().SingleInstance();

            builder.RegisterType<TaskService>().As<ITaskService>().SingleInstance();
            builder.RegisterType<CategoryService>().As<ICategoryService>().SingleInstance();

            builder.RegisterType<MainForm>().AsSelf();
            builder.RegisterType<TaskEditForm>().AsSelf();
            builder.RegisterType<CategoryForm>().AsSelf();

            Container = builder.Build();

            try
            {
                ILogger logger = Log.ForContext("SourceContext", nameof(Program));
                logger.Information("TodoApp has been launched successfully.");

                using (ILifetimeScope scope = Container.BeginLifetimeScope())
                {
                    MainForm mainForm = scope.Resolve<MainForm>();
                    Application.Run(mainForm);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Critical error while starting the application.");
            }
            finally
            {
                ILogger logger = Log.ForContext("SourceContext", nameof(Program));
                logger.Information("TodoApp has been closed.");

                // Ensures that all logs from the buffer are written to disk.
                Log.CloseAndFlush();
            }
        }
    }
}
