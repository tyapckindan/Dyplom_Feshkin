using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dyplom_Feshkin
{
    public partial class Log_in_Form : Form
    {
        readonly string connectionString = @"Data source=DESKTOP-FJFCP05;Initial Catalog=MedicineWarehouse;Integrated Security=True";
        public Log_in_Form()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen; // Открытие формы по центру экран
        }

        private void Log_in_Form_Load(object sender, EventArgs e)
        {
            textBox_password.UseSystemPasswordChar = true;
            pictureBox1.Visible = false;
            textBox_login.MaxLength = 50;
            textBox_password.MaxLength = 50;
        }

        private void Enter_Btn_Click(object sender, EventArgs e)
        {
            if (textBox_password.TextLength != 0 && textBox_password.TextLength != 0)
            {
                var logUser = textBox_login.Text;
                var passUser = textBox_password.Text;

                SqlDataAdapter adapter = new SqlDataAdapter();
                DataTable table = new DataTable();

                string querystring = "SELECT * FROM register WHERE login_user = @login AND password_user = @password";

                SqlCommand connection = new SqlCommand(querystring, new SqlConnection(connectionString));
                connection.Parameters.AddWithValue("@login", logUser);
                connection.Parameters.AddWithValue("@password", passUser);

                adapter.SelectCommand = connection;
                adapter.Fill(table);

                if (table.Rows.Count == 1)
                {
                    var user = new checkUser(table.Rows[0].ItemArray[1].ToString(), Convert.ToBoolean(table.Rows[0].ItemArray[3].ToString()));

                    MessageBox.Show("Вы успешно вошли!", "Успешно!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MainForm frm1 = new MainForm(user);
                    Hide();
                    frm1.ShowDialog();
                    Show();
                }
                else
                {
                    MessageBox.Show("Такого аккаунта не существует!", "Аккаунта не существует!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Заполните все поля!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClearClick(object sender, EventArgs e)
        {
            textBox_login.Text = "";
            textBox_password.Text = "";
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            textBox_password.UseSystemPasswordChar = true;
            pictureBox1.Visible = false;
            pictureBox2.Visible = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            textBox_password.UseSystemPasswordChar = false;
            pictureBox1.Visible = true;
            pictureBox2.Visible = false;
        }
    }
}