using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using TodoApp.WinForms.Models;
using TodoApp.WinForms.Services;
using TodoApp.WinForms.Services.Abstractions;

namespace TodoApp.WinForms.Views
{
    public partial class CategoryForm : Form
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;

        public CategoryForm(ICategoryService categoryService, ILogger logger)
        {
            InitializeComponent();

            _categoryService = categoryService;
            _logger = logger.ForContext<CategoryForm>();

            // Setup window preferences
            StartPosition = FormStartPosition.CenterParent;

            // Wire events
            Load += CategoryForm_Load;
            btnAddCategory.Click += BtnAddCategory_Click;
            btnDeleteCategory.Click += BtnDeleteCategory_Click;
        }

        private async void CategoryForm_Load(object sender, EventArgs e)
        {
            _logger.Information("Loading CategoryForm UI.");
            await RefreshCategoryListAsync();
        }

        private async Task RefreshCategoryListAsync()
        {
            try
            {
                dgvCategories.Enabled = false;

                // Fetch categories asynchronously from the service layer
                var categories = await _categoryService.GetAllCategoriesAsync();

                dgvCategories.DataSource = null;
                dgvCategories.DataSource = categories.ToList();

                ConfigureGridColumns();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to refresh category list.");
                MessageBox.Show("Error loading categories from database.", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dgvCategories.Enabled = true;
            }
        }

        private void ConfigureGridColumns()
        {
            if (dgvCategories.Columns.Count == 0) return;

            // Hide the technical ID column
            if (dgvCategories.Columns["Id"] != null)
                dgvCategories.Columns["Id"].Visible = false;

            // Header styling
            if (dgvCategories.Columns["Name"] != null)
            {
                dgvCategories.Columns["Name"].HeaderText = "Category Name";
                dgvCategories.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            dgvCategories.RowHeadersVisible = false;
        }

        private async void BtnAddCategory_Click(object sender, EventArgs e)
        {
            string categoryName = txtCategoryName.Text.Trim();

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                _logger.Warning("User tried to add a category with an empty name.");
                MessageBox.Show("Category name cannot be empty.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _logger.Information("Attempting to create a new category: {CategoryName}", categoryName);

                var newCategory = new CategoryEntity { Name = categoryName };
                await _categoryService.CreateCategoryAsync(newCategory);

                txtCategoryName.Clear();
                _logger.Information("Category '{CategoryName}' created successfully.", categoryName);

                await RefreshCategoryListAsync();
            }
            catch (ArgumentException ex)
            {
                _logger.Warning("Validation failed on BLL level: {Message}", ex.Message);
                MessageBox.Show(ex.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error while adding a category.");
                MessageBox.Show("Failed to save the category.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteCategory_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow?.DataBoundItem is CategoryEntity selectedCategory)
            {
                var dialogResult = MessageBox.Show(
                    $"Are you sure you want to delete the category '{selectedCategory.Name}'?\n" +
                    "All associated tasks will be reassigned to 'No Category'.",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    try
                    {
                        _logger.Information("User requested deletion of Category ID: {CategoryId}", selectedCategory.Id);

                        await _categoryService.DeleteCategoryAsync(selectedCategory.Id);

                        _logger.Information("Category ID: {CategoryId} successfully deleted.", selectedCategory.Id);
                        await RefreshCategoryListAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error occurred while deleting category ID: {CategoryId}", selectedCategory.Id);
                        MessageBox.Show("Could not delete the category. It might be locked by the system.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a category to delete.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
