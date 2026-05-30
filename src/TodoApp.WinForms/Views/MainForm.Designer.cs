namespace TodoApp.WinForms.Views
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.btnManageCategories = new System.Windows.Forms.Button();
            this.lblCategoryFilter = new System.Windows.Forms.Label();
            this.cmbFilterCategory = new System.Windows.Forms.ComboBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.dgvTasks = new System.Windows.Forms.DataGridView();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dgvcTaskName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcCategoryName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcPriorityName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcStatusName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcDueDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvcCreatedAt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlTop.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).BeginInit();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.btnManageCategories);
            this.pnlTop.Controls.Add(this.lblCategoryFilter);
            this.pnlTop.Controls.Add(this.cmbFilterCategory);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1234, 35);
            this.pnlTop.TabIndex = 0;
            // 
            // btnManageCategories
            // 
            this.btnManageCategories.Location = new System.Drawing.Point(367, 3);
            this.btnManageCategories.Name = "btnManageCategories";
            this.btnManageCategories.Size = new System.Drawing.Size(143, 28);
            this.btnManageCategories.TabIndex = 2;
            this.btnManageCategories.Text = "Manage categories";
            this.btnManageCategories.UseVisualStyleBackColor = true;
            this.btnManageCategories.Click += new System.EventHandler(this.btnManageCategories_Click);
            // 
            // lblCategoryFilter
            // 
            this.lblCategoryFilter.AutoSize = true;
            this.lblCategoryFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCategoryFilter.Location = new System.Drawing.Point(4, 9);
            this.lblCategoryFilter.Name = "lblCategoryFilter";
            this.lblCategoryFilter.Size = new System.Drawing.Size(124, 16);
            this.lblCategoryFilter.TabIndex = 1;
            this.lblCategoryFilter.Text = "Filter by categories:";
            // 
            // cmbFilterCategory
            // 
            this.cmbFilterCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilterCategory.FormattingEnabled = true;
            this.cmbFilterCategory.Location = new System.Drawing.Point(153, 6);
            this.cmbFilterCategory.Name = "cmbFilterCategory";
            this.cmbFilterCategory.Size = new System.Drawing.Size(208, 24);
            this.cmbFilterCategory.TabIndex = 0;
            this.cmbFilterCategory.SelectedIndexChanged += new System.EventHandler(this.CmbFilterCategory_SelectedIndexChanged);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 424);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1234, 26);
            this.statusStrip.TabIndex = 1;
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(45, 20);
            this.lblStatus.Text = "Done";
            // 
            // dgvTasks
            // 
            this.dgvTasks.AllowUserToAddRows = false;
            this.dgvTasks.AllowUserToDeleteRows = false;
            this.dgvTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTasks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvcTaskName,
            this.dgvcDescription,
            this.dgvcCategoryName,
            this.dgvcPriorityName,
            this.dgvcStatusName,
            this.dgvcDueDate,
            this.dgvcCreatedAt});
            this.dgvTasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTasks.Location = new System.Drawing.Point(0, 35);
            this.dgvTasks.Name = "dgvTasks";
            this.dgvTasks.ReadOnly = true;
            this.dgvTasks.RowHeadersVisible = false;
            this.dgvTasks.RowHeadersWidth = 51;
            this.dgvTasks.RowTemplate.Height = 24;
            this.dgvTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTasks.Size = new System.Drawing.Size(1034, 389);
            this.dgvTasks.TabIndex = 2;
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.btnDelete);
            this.pnlActions.Controls.Add(this.btnEdit);
            this.pnlActions.Controls.Add(this.btnAdd);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlActions.Location = new System.Drawing.Point(1034, 35);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(200, 389);
            this.pnlActions.TabIndex = 3;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(18, 64);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(170, 23);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(18, 35);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(170, 23);
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(18, 6);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(170, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dgvcTaskName
            // 
            this.dgvcTaskName.DataPropertyName = "Name";
            this.dgvcTaskName.HeaderText = "Task name";
            this.dgvcTaskName.MinimumWidth = 6;
            this.dgvcTaskName.Name = "dgvcTaskName";
            this.dgvcTaskName.ReadOnly = true;
            this.dgvcTaskName.Width = 125;
            // 
            // dgvcDescription
            // 
            this.dgvcDescription.DataPropertyName = "Description";
            this.dgvcDescription.HeaderText = "Description";
            this.dgvcDescription.MinimumWidth = 6;
            this.dgvcDescription.Name = "dgvcDescription";
            this.dgvcDescription.ReadOnly = true;
            this.dgvcDescription.Width = 125;
            // 
            // dgvcCategoryName
            // 
            this.dgvcCategoryName.DataPropertyName = "CategoryName";
            this.dgvcCategoryName.FillWeight = 25F;
            this.dgvcCategoryName.HeaderText = "Category";
            this.dgvcCategoryName.MinimumWidth = 6;
            this.dgvcCategoryName.Name = "dgvcCategoryName";
            this.dgvcCategoryName.ReadOnly = true;
            this.dgvcCategoryName.Width = 80;
            // 
            // dgvcPriorityName
            // 
            this.dgvcPriorityName.DataPropertyName = "PriorityName";
            this.dgvcPriorityName.FillWeight = 25F;
            this.dgvcPriorityName.HeaderText = "Priority";
            this.dgvcPriorityName.MinimumWidth = 6;
            this.dgvcPriorityName.Name = "dgvcPriorityName";
            this.dgvcPriorityName.ReadOnly = true;
            this.dgvcPriorityName.Width = 80;
            // 
            // dgvcStatusName
            // 
            this.dgvcStatusName.DataPropertyName = "StatusName";
            this.dgvcStatusName.FillWeight = 25F;
            this.dgvcStatusName.HeaderText = "Status";
            this.dgvcStatusName.MinimumWidth = 6;
            this.dgvcStatusName.Name = "dgvcStatusName";
            this.dgvcStatusName.ReadOnly = true;
            this.dgvcStatusName.Width = 80;
            // 
            // dgvcDueDate
            // 
            this.dgvcDueDate.DataPropertyName = "DueDate";
            dataGridViewCellStyle1.Format = "dd.MM.yyyy";
            this.dgvcDueDate.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvcDueDate.HeaderText = "Due date";
            this.dgvcDueDate.MinimumWidth = 6;
            this.dgvcDueDate.Name = "dgvcDueDate";
            this.dgvcDueDate.ReadOnly = true;
            this.dgvcDueDate.Width = 125;
            // 
            // dgvcCreatedAt
            // 
            this.dgvcCreatedAt.DataPropertyName = "CreatedAt";
            dataGridViewCellStyle2.Format = "dd.MM.yyyy HH:mm";
            this.dgvcCreatedAt.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvcCreatedAt.HeaderText = "Created at";
            this.dgvcCreatedAt.MinimumWidth = 6;
            this.dgvcCreatedAt.Name = "dgvcCreatedAt";
            this.dgvcCreatedAt.ReadOnly = true;
            this.dgvcCreatedAt.Width = 125;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 450);
            this.Controls.Add(this.dgvTasks);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.pnlTop);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TodoApp";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).EndInit();
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblCategoryFilter;
        private System.Windows.Forms.ComboBox cmbFilterCategory;
        private System.Windows.Forms.Button btnManageCategories;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.DataGridView dgvTasks;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcTaskName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcCategoryName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcPriorityName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcStatusName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcDueDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvcCreatedAt;
    }
}

