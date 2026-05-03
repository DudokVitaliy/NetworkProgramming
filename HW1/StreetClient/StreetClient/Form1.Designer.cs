namespace StreetClient
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private TextBox textBoxIndex;
        private Button buttonSend;
        private ListBox listBoxStreets;
        private Label label1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            textBoxIndex = new TextBox();
            buttonSend = new Button();
            listBoxStreets = new ListBox();
            label1 = new Label();
            SuspendLayout();

            label1.AutoSize = true;
            label1.Location = new Point(30, 30);
            label1.Name = "label1";
            label1.Size = new Size(120, 15);
            label1.Text = "Поштовий індекс:";

            textBoxIndex.Location = new Point(30, 55);
            textBoxIndex.Name = "textBoxIndex";
            textBoxIndex.Size = new Size(200, 23);

            buttonSend.Location = new Point(250, 55);
            buttonSend.Name = "buttonSend";
            buttonSend.Size = new Size(150, 25);
            buttonSend.Text = "Отримати вулиці";
            buttonSend.UseVisualStyleBackColor = true;
            buttonSend.Click += buttonSend_Click;

            listBoxStreets.Location = new Point(30, 100);
            listBoxStreets.Name = "listBoxStreets";
            listBoxStreets.Size = new Size(370, 200);

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(450, 350);
            Controls.Add(label1);
            Controls.Add(textBoxIndex);
            Controls.Add(buttonSend);
            Controls.Add(listBoxStreets);
            Name = "Form1";
            Text = "Пошук вулиць за індексом";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}