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
    public partial class MainForm : Form
    {
        readonly string connectionString = @"Data source=DESKTOP-FJFCP05;Initial Catalog=MedicineWarehouse;Integrated Security=True";

        private readonly checkUser _user;

        string useTable = "Delivery";

        private void IsAdmin()
        {
            dataGridView1.ReadOnly = !_user.IsAdmin;
        }

        public MainForm(checkUser user)
        {
            _user = user;
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen; // Открытие формы по центру экрана

            // Убираем стандартное меню из дизайна
            menuStrip1.Items.Clear();

            // Добавляем пункты меню с иконками
            AddMenuItem("Поставки", "addMenuStripDelivery.jpg", MenuStripDelivery);
            AddMenuItem("Товары", "addMenuStripNewProduct.jpg", MenuStripNewProducts);
            AddMenuItem("Склады", "addMenuStripWarehouses.jpg", MenuStripWarehouses);
            AddMenuItem("Поставщики", "addMenuStripSuppliers.jpg", MenuStripSuppliers);
            AddMenuItem("Сохранить изменения", "addMenuStripSaveData.jpg", MenuStripSaveData);
            AddMenuItem("Администрирование", "addMenuStripAdmin.jpg", MenuStripAdmin);

            LoadData(useTable);
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

        // Элементы меню
        private void MenuStripDelivery(object sender, EventArgs e)
        {
            Text = "Поставки"; 
            useTable = "Delivery";
            LoadData(useTable);
        }
        private void MenuStripNewProducts(object sender, EventArgs e)
        {
            Text = "Товары";
            useTable = "Product";
            LoadData(useTable);
        }
        private void MenuStripWarehouses(object sender, EventArgs e)
        {
            Text = "Склады";
            useTable = "Warehouse";
            LoadData(useTable);
        }
        private void MenuStripSuppliers(object sender, EventArgs e)
        {
            Text = "Поставщики";
            useTable = "Supplier";
            LoadData(useTable);
        }
        private void MenuStripSaveData(object sender, EventArgs e)
        {
            if (_user.IsAdmin)
            {
                SaveData(useTable);
            }
            else
            {
                MessageBox.Show("У вас нет прав на изменение данных в таблицах!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void MenuStripAdmin(object sender, EventArgs e)
        {
            if (_user.IsAdmin)
            {
                Panel_admin adm = new Panel_admin();
            Hide();
            adm.ShowDialog();
            Show();
            }
            else
            {
                MessageBox.Show("У вас нет прав на администрирование!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IsAdmin();
            LoadData(useTable);
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
            string query = "";

            // Таблица товары
            if (tableName == "Product")
             query = $"SELECT {tableName}id, {tableName}Name AS [Наименование товара], {tableName}Category AS [Категория], {tableName}Description AS [Описание], {tableName}UnitOfMeasure AS [ед. измерения], {tableName}StorageConditions AS [Условия хранения], {tableName}RequiresPrescription AS [Требуется рецепт врача] FROM {tableName}";

            // Таблица Склады 
            if (tableName == "Warehouse")
                query = $"SELECT {tableName}id, {tableName}Name AS [Наименование склада], {tableName}Address AS [Адрес], {tableName}ContactPerson AS [Контактное лицо], {tableName}TemperatureControl AS [Контроль температуры], {tableName}HumidityControl AS [Контроль влажности], {tableName}FireSafetyCertified AS [Сертификация пожаробезопасности] FROM {tableName}";

            // Таблица поставщики
            if (tableName == "Supplier")
                query = $"SELECT {tableName}id, {tableName}Name AS [Наименование поставщика], {tableName}Address AS [Адрес], {tableName}Phone AS [Телефон], {tableName}Email, {tableName}ContactPerson AS [Контактное лицо], {tableName}HasMedicalLicense AS [Лицензия на продажу медицинских товаров], {tableName}CertifiedSupplier AS [Сертифицированный поставщик] FROM {tableName}";

            // Таблица Новая поставка 
            if (tableName == "Delivery")
                query = $"SELECT {tableName}id, ProductName AS [Наименование товара], WarehouseName AS [Наименование склада], SupplierName AS [Наименование поставщика], Description AS [Описание], Quantity AS [Количество], Price AS [Цена], DeliveryDate AS [Дата поставки], ExpiryDate [Истечение срока годности], BatchNumber AS [Номер партии]  FROM {tableName}";

            return query;
        }
    }
}