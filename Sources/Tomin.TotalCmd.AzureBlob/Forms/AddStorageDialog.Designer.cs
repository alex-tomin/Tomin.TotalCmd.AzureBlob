namespace Tomin.TotalCmd.AzureBlob.Forms
{
	partial class AddStorageDialog
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
			this.components = new System.ComponentModel.Container();
			this.displayNameLabel = new System.Windows.Forms.Label();
			this.displayNameTextBox = new System.Windows.Forms.TextBox();
			this.accountNameTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.accountKeyTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.useSslCheckBox = new System.Windows.Forms.CheckBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.addButton = new System.Windows.Forms.Button();
			this.developmentStorageCheckBox = new System.Windows.Forms.CheckBox();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// displayNameLabel
			// 
			this.displayNameLabel.AutoSize = true;
			this.displayNameLabel.Location = new System.Drawing.Point(28, 80);
			this.displayNameLabel.Name = "displayNameLabel";
			this.displayNameLabel.Size = new System.Drawing.Size(76, 13);
			this.displayNameLabel.TabIndex = 0;
			this.displayNameLabel.Text = "Display Name*";
			// 
			// displayNameTextBox
			// 
			this.displayNameTextBox.Location = new System.Drawing.Point(31, 96);
			this.displayNameTextBox.Name = "displayNameTextBox";
			this.displayNameTextBox.Size = new System.Drawing.Size(227, 20);
			this.displayNameTextBox.TabIndex = 3;
			this.displayNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
			// 
			// accountNameTextBox
			// 
			this.accountNameTextBox.Location = new System.Drawing.Point(31, 47);
			this.accountNameTextBox.Name = "accountNameTextBox";
			this.accountNameTextBox.Size = new System.Drawing.Size(227, 20);
			this.accountNameTextBox.TabIndex = 1;
			this.accountNameTextBox.Leave += new System.EventHandler(this.accountNameTextBox_Leave);
			this.accountNameTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(28, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Account Name*";
			// 
			// accountKeyTextBox
			// 
			this.accountKeyTextBox.Location = new System.Drawing.Point(31, 154);
			this.accountKeyTextBox.Multiline = true;
			this.accountKeyTextBox.Name = "accountKeyTextBox";
			this.accountKeyTextBox.Size = new System.Drawing.Size(343, 54);
			this.accountKeyTextBox.TabIndex = 5;
			this.accountKeyTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(28, 138);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Account Key*";
			// 
			// useSslCheckBox
			// 
			this.useSslCheckBox.AutoSize = true;
			this.useSslCheckBox.Checked = true;
			this.useSslCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.useSslCheckBox.Location = new System.Drawing.Point(306, 99);
			this.useSslCheckBox.Name = "useSslCheckBox";
			this.useSslCheckBox.Size = new System.Drawing.Size(68, 17);
			this.useSslCheckBox.TabIndex = 7;
			this.useSslCheckBox.Text = "Use SSL";
			this.useSslCheckBox.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(195, 251);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 8;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// addButton
			// 
			this.addButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.addButton.Location = new System.Drawing.Point(299, 251);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(75, 23);
			this.addButton.TabIndex = 9;
			this.addButton.Text = "Add Storage";
			this.addButton.UseVisualStyleBackColor = true;
			// 
			// developmentStorageCheckBox
			// 
			this.developmentStorageCheckBox.AutoSize = true;
			this.developmentStorageCheckBox.Location = new System.Drawing.Point(31, 225);
			this.developmentStorageCheckBox.Name = "developmentStorageCheckBox";
			this.developmentStorageCheckBox.Size = new System.Drawing.Size(179, 17);
			this.developmentStorageCheckBox.TabIndex = 10;
			this.developmentStorageCheckBox.Text = "Development Storage (Emulator)";
			this.developmentStorageCheckBox.UseVisualStyleBackColor = true;
			this.developmentStorageCheckBox.CheckedChanged += new System.EventHandler(this.developmentStorageCheckBox_CheckedChanged);
			// 
			// errorProvider
			// 
			this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
			this.errorProvider.ContainerControl = this;
			// 
			// AddStorageDialog
			// 
			this.AcceptButton = this.addButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(416, 301);
			this.Controls.Add(this.developmentStorageCheckBox);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.useSslCheckBox);
			this.Controls.Add(this.accountKeyTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.accountNameTextBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.displayNameTextBox);
			this.Controls.Add(this.displayNameLabel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AddStorageDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add Blob Storage";
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label displayNameLabel;
		private System.Windows.Forms.TextBox displayNameTextBox;
		private System.Windows.Forms.TextBox accountNameTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox accountKeyTextBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox useSslCheckBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.CheckBox developmentStorageCheckBox;
		private System.Windows.Forms.ErrorProvider errorProvider;
	}
}