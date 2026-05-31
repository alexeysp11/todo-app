using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autofac;
using Serilog;
using TodoApp.WinForms.Models;
using TodoApp.WinForms.Services.Abstractions;

namespace TodoApp.WinForms.Views
{
    public partial class MainForm : Form
    {
        private readonly ITaskService _taskService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;

        public MainForm(ILogger logger, ITaskService taskService, ICategoryService categoryService)
        {
            InitializeComponent();
            _logger = logger.ForContext<MainForm>();
            _taskService = taskService;
            _categoryService = categoryService;

            Load += MainForm_Load;
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                _logger.Information("Loading the main interface form.");
                dgvTasks.AutoGenerateColumns = false;
                await Task.WhenAll(LoadCategoriesFilterAsync(), RefreshTaskListAsync());
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Critical failure during main form initialization.");
                MessageBox.Show("Failed to initialize application interface.", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task RefreshTaskListAsync()
        {
            try
            {
                dgvTasks.Enabled = false;
                statusStrip.Text = "Loading tasks from database...";

                IEnumerable<TodoTaskEntity> tasks;

                // Fetch by category if specific filter is active, otherwise fetch all
                if (cmbFilterCategory.SelectedValue is int selectedCategoryId && selectedCategoryId > 0)
                {
                    tasks = await _taskService.GetByCategoryIdAsync(selectedCategoryId);
                }
                else
                {
                    tasks = await _taskService.GetAllTasksAsync();
                }

                dgvTasks.DataSource = tasks.ToList();
                statusStrip.Text = $"Total tasks: {dgvTasks.Rows.Count}";
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Critical error updating task list.");
                statusStrip.Text = "Error loading data.";
                throw;
            }
            finally
            {
                dgvTasks.Enabled = true;
            }
        }

        private async Task LoadCategoriesFilterAsync()
        {
            try
            {
                List<CategoryEntity> categories = new List<CategoryEntity>
            {
                new CategoryEntity { Id = 0, Name = "All Categories" }
            };

                IEnumerable<CategoryEntity> dbCategories = await _categoryService.GetAllCategoriesAsync();
                categories.AddRange(dbCategories);

                cmbFilterCategory.DataSource = categories;
                cmbFilterCategory.DisplayMember = nameof(CategoryEntity.Name);
                cmbFilterCategory.ValueMember = nameof(CategoryEntity.Id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error loading category filter.");
                throw;
            }
        }

        private async void CmbFilterCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Additional protection: ignoring false positives when initializing a combo box
                if (cmbFilterCategory.SelectedValue == null) return;

                await RefreshTaskListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error triggered by filter category change.");
                MessageBox.Show("Failed to update task list after filter change.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                // Checking if a row is selected in the grid
                if (dgvTasks.CurrentRow?.DataBoundItem is TodoTaskEntity selectedTask)
                {
                    DialogResult result = MessageBox.Show($"Are you sure you want to delete task '{selectedTask.Name}'?",
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        _logger.Information("User deletes task with ID: {TaskId}", selectedTask.Id);
                        await _taskService.DeleteTaskAsync(selectedTask.Id);
                        await RefreshTaskListAsync();
                    }
                }
                else
                {
                    MessageBox.Show("Please select a task to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to delete task");
                MessageBox.Show("Could not delete the task. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (ILifetimeScope scope = Program.Container.BeginLifetimeScope())
                {
                    TaskEditForm editForm = scope.Resolve<TaskEditForm>();
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        await RefreshTaskListAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Critical error opening {nameof(TaskEditForm)} for adding a new task.");
                MessageBox.Show("Failed to open the creation form.", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(dgvTasks.CurrentRow?.DataBoundItem is TodoTaskEntity selectedTask))
                {
                    _logger.Warning("User tried to edit a task without selecting one.");
                    MessageBox.Show("Please select a task to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _logger.Information("Opening TaskEditForm in edit mode for Task ID: {TaskId}", selectedTask.Id);

                using (ILifetimeScope scope = Program.Container.BeginLifetimeScope())
                {
                    TaskEditForm editForm = scope.Resolve<TaskEditForm>(new TypedParameter(typeof(TodoTaskEntity), selectedTask));

                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        await RefreshTaskListAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Critical error opening or processing {nameof(TaskEditForm)}.");
                MessageBox.Show("Failed to open the edit form due to an internal error.", "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnManageCategories_Click(object sender, EventArgs e)
        {
            _logger.Information("User opened Category Management window.");
            try
            {
                using (ILifetimeScope scope = Program.Container.BeginLifetimeScope())
                {
                    CategoryForm categoryForm = scope.Resolve<CategoryForm>();
                    categoryForm.ShowDialog();
                }
                await LoadCategoriesFilterAsync();
                
                // Refresh task list as categories may have been altered or deleted
                await RefreshTaskListAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to open Category Management window.");
                MessageBox.Show("Failed to manage categories.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
