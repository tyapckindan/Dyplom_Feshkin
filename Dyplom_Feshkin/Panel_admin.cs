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
    public partial class Panel_admin : Form
    {
        readonly string connectionString = @"Data source=DESKTOP-FJFCP05;Initial Catalog=MedicineWarehouse;Integrated Security=True";

        string useTable = "register";
        public Panel_admin()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            // Убираем стандартное меню из дизайна
            menuStrip1.Items.Clear();

            AddMenuItem("Сохранить изменения", "addMenuStripSaveData.jpg", MenuStripSaveData);
        }

        // Метод для добавления картинки в меню
        private void AddMenuItem(string text, string imagePath, EventHandler handler)
        {
            var item = new ToolStripMenuItem(text);

            try
            {
                item.Image = Image.FromFile(imagePath);
            }
            catch
            {
                MessageBox.Show($"Не найдено изображение: {imagePath}");
            }

            item.Click += handler;
            item.AutoSize = false;
            item.Size = new Size(150, 30); // Можно настроить размер
            item.TextAlign = ContentAlignment.MiddleLeft;

            menuStrip1.Items.Add(item);
        }

        // Загрузка данных
        private void LoadData(string tableName)
        {
            // Очищаем старые колонки, если нужно
            dataGridView1.Columns.Clear();

            string query = CreateQuerySQL(tableName);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                System.Data.DataTable table = new System.Data.DataTable();
                adapter.Fill(table);

                // Привязываем к dataGridView
                dataGridView1.DataSource = table;

                // Автоподбор ширины и высоты
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                // Скрываем полe id, т.к. пользователю не нужно с ним работать
                if (dataGridView1.Columns.Contains($"{tableName}id"))
                {
                    dataGridView1.Columns[$"{tableName}id"].Visible = false;
                }
            }
        }

        // Метод сохранения данных
        private void MenuStripSaveData(object sender, EventArgs e)
        {
            SaveData(useTable);
        }

        // Метод сохранения изменений в таблицах
        private void SaveData(string tableName)
        {
            string query = CreateQuerySQL(tableName);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);

                // Для INSERT, UPDATE, DELETE создаём команды
                SqlCommandBuilder builder = new SqlCommandBuilder(adapter);

                System.Data.DataTable table = dataGridView1.DataSource as System.Data.DataTable;

                if (table != null)
                {
                    try
                    {
                        adapter.Update(table);
                        MessageBox.Show("Изменения успешно сохранены!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("Не удалось получить данные из таблицы.");
                }
            }
        }

        // Метод для создания Sql запроса
        private string CreateQuerySQL(string tableName)
        {
            string query = $"SELECT * FROM {tableName}";

            return query;
        }

        private void Panel_admin_Load(object sender, EventArgs e)
        {
            LoadData(useTable);
        }
    }
}
