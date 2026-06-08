using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace Proiect_MSOA___nr_5
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        // 1. ECRANUL PROGRAMARI
        private void BtnProgramari_Click(object sender, RoutedEventArgs e)
        {
            ActualizeazaTabelProgramari();
        }

        private void ActualizeazaTabelProgramari()
        {
            try
            {
                using (var db = new BazaDeDateContext())
                {
                    DataGrid dg = CreeazaDataGridStilizat();
                    dg.Columns.Add(new DataGridTextColumn { Header = "ID Rezervare", Binding = new Binding("Id"), Width = new DataGridLength(90) });
                    dg.Columns.Add(new DataGridTextColumn { Header = "ID Client", Binding = new Binding("IdClient"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
                    dg.Columns.Add(new DataGridTextColumn { Header = "ID Angajat", Binding = new Binding("IdAngajat"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
                    dg.Columns.Add(new DataGridTextColumn { Header = "ID Serviciu", Binding = new Binding("IdServiciu"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
                    dg.Columns.Add(new DataGridTextColumn { Header = "Data & Ora", Binding = new Binding("DataOra") { StringFormat = "{0:dd.MM.yyyy HH:mm}" }, Width = new DataGridLength(140) });
                    dg.Columns.Add(new DataGridTextColumn { Header = "Status", Binding = new Binding("Status"), Width = new DataGridLength(110) });

                    dg.ItemsSource = db.Programari.ToList();

                    StackPanel sp = new StackPanel { Margin = new Thickness(10) };
                    sp.Children.Add(CreeazaTitluEcran("📅 Gestiune Programări Active"));
                    sp.Children.Add(dg);

                    Button btnSterge = CreeazaButonActiune("❌ Șterge Programarea Selectată", Colors.DarkRed);
                    btnSterge.Click += (s, args) =>
                    {
                        var selectat = dg.SelectedItem as Programare;
                        if (selectat != null)
                        {
                            var resp = MessageBox.Show($"Sigur doriți să ștergeți programarea cu ID-ul {selectat.Id}?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (resp == MessageBoxResult.Yes)
                            {
                                using (var dbContextNou = new BazaDeDateContext())
                                {
                                    var pDeSters = dbContextNou.Programari.Find(selectat.Id);
                                    if (pDeSters != null)
                                    {
                                        dbContextNou.Programari.Remove(pDeSters);
                                        dbContextNou.SaveChanges();
                                        MessageBox.Show("Programarea a fost eliminată cu succes!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                                    }
                                }
                                ActualizeazaTabelProgramari();
                            }
                        }
                        else { MessageBox.Show("Selectați mai întâi o programare din tabel!", "Atenție"); }
                    };
                    sp.Children.Add(btnSterge);
                    ZonaCentrala.Content = sp;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // 2. ECRANUL UTILIZATORI
        private void BtnUtilizatori_Click(object sender, RoutedEventArgs e)
        {
            ActualizeazaTabelUtilizatori();
        }

        private void ActualizeazaTabelUtilizatori()
        {
            using (var db = new BazaDeDateContext())
            {
                DataGrid dg = CreeazaDataGridStilizat();
                dg.Columns.Add(new DataGridTextColumn { Header = "ID Cont", Binding = new Binding("Id"), Width = new DataGridLength(70) });
                dg.Columns.Add(new DataGridTextColumn { Header = "Nume Utilizator (Username)", Binding = new Binding("Username"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
                dg.Columns.Add(new DataGridTextColumn { Header = "Rol Sistem", Binding = new Binding("Rol"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });

                dg.ItemsSource = db.Utilizatori.ToList();

                StackPanel sp = new StackPanel { Margin = new Thickness(10) };
                sp.Children.Add(CreeazaTitluEcran("👥 Management Utilizatori și Staff (Securizat)"));
                sp.Children.Add(dg);

                Grid panelButoane = new Grid { Margin = new Thickness(0, 20, 0, 0) };
                panelButoane.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                panelButoane.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Button btnAdauga = CreeazaButonActiune("➕ Adaugă Utilizator Nou", Colors.DarkGreen);
                btnAdauga.HorizontalAlignment = HorizontalAlignment.Left;
                btnAdauga.Click += (s, args) =>
                {
                    Window wDialog = new Window { Title = "Adaugă Utilizator Nou", Width = 320, Height = 320, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)), Foreground = Brushes.White, ResizeMode = ResizeMode.NoResize };
                    StackPanel panel = new StackPanel { Margin = new Thickness(20) };
                    panel.Children.Add(new TextBlock { Text = "Username:", Margin = new Thickness(0, 5, 0, 2), FontWeight = FontWeights.Bold });
                    TextBox txtUser = new TextBox { Height = 25, Margin = new Thickness(0, 0, 0, 10), Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), Foreground = Brushes.White, BorderBrush = Brushes.Gray }; panel.Children.Add(txtUser);
                    panel.Children.Add(new TextBlock { Text = "Parolă Inițială:", Margin = new Thickness(0, 5, 0, 2), FontWeight = FontWeights.Bold });
                    TextBox txtPass = new TextBox { Height = 25, Margin = new Thickness(0, 0, 0, 10), Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), Foreground = Brushes.White, BorderBrush = Brushes.Gray }; panel.Children.Add(txtPass);
                    panel.Children.Add(new TextBlock { Text = "Rol (Administrator/Angajat/Client):", Margin = new Thickness(0, 5, 0, 2), FontWeight = FontWeights.Bold });
                    ComboBox cbRol = new ComboBox { Height = 25, Margin = new Thickness(0, 0, 0, 20) }; cbRol.Items.Add("Administrator"); cbRol.Items.Add("Angajat"); cbRol.Items.Add("Client"); cbRol.SelectedIndex = 1; panel.Children.Add(cbRol);

                    Button btnSalva = new Button { Content = "🟢 Salvează în Baza de Date", Height = 35, Background = Brushes.DarkGreen, Foreground = Brushes.White, FontWeight = FontWeights.Bold, BorderBrush = Brushes.Transparent };
                    btnSalva.Click += (sDel, argsDel) =>
                    {
                        if (string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtPass.Text)) { MessageBox.Show("Completați toate câmpurile!", "Atenție"); return; }
                        using (var dbCtx = new BazaDeDateContext())
                        {
                            dbCtx.Utilizatori.Add(new Utilizator { Username = txtUser.Text, Parola = txtPass.Text, Rol = cbRol.SelectedItem.ToString() });
                            dbCtx.SaveChanges();
                        }
                        MessageBox.Show("Utilizator adăugat cu succes!", "Succes"); wDialog.Close(); ActualizeazaTabelUtilizatori();
                    };
                    panel.Children.Add(btnSalva); wDialog.Content = panel; wDialog.ShowDialog();
                };

                Button btnStergeUser = CreeazaButonActiune("🗑️ Șterge Utilizatorul Selectat", Colors.DarkRed);
                btnStergeUser.HorizontalAlignment = HorizontalAlignment.Right;
                btnStergeUser.Click += (s, args) =>
                {
                    var selectat = dg.SelectedItem as Utilizator;
                    if (selectat != null)
                    {
                        if (selectat.Username.ToLower() == "admin") { MessageBox.Show("Contul principal 'admin' nu poate fi șters!", "Eroare"); return; }
                        var resp = MessageBox.Show($"Sigur doriți să ștergeți contul utilizatorului '{selectat.Username}'?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (resp == MessageBoxResult.Yes)
                        {
                            using (var dbCtx = new BazaDeDateContext())
                            {
                                var uDeSters = dbCtx.Utilizatori.Find(selectat.Id);
                                if (uDeSters != null) { dbCtx.Utilizatori.Remove(uDeSters); dbCtx.SaveChanges(); MessageBox.Show("Utilizator șters!"); }
                            }
                            ActualizeazaTabelUtilizatori();
                        }
                    }
                    else { MessageBox.Show("Selectați un utilizator!"); }
                };

                Grid.SetColumn(btnAdauga, 0); panelButoane.Children.Add(btnAdauga);
                Grid.SetColumn(btnStergeUser, 1); panelButoane.Children.Add(btnStergeUser);
                sp.Children.Add(panelButoane);
                ZonaCentrala.Content = sp;
            }
        }

        // 3. ECRANUL PRODUSE & SERVICII
        private void BtnProduse_Click(object sender, RoutedEventArgs e)
        {
            ActualizeazaTabelProduse();
        }

        private void ActualizeazaTabelProduse()
        {
            using (var db = new BazaDeDateContext())
            {
                DataGrid dg = CreeazaDataGridStilizat();
                dg.Columns.Add(new DataGridTextColumn { Header = "ID", Binding = new Binding("Id"), Width = new DataGridLength(60) });
                dg.Columns.Add(new DataGridTextColumn { Header = "Denumire Produs / Serviciu", Binding = new Binding("Denumire"), Width = new DataGridLength(2, DataGridLengthUnitType.Star) });
                dg.Columns.Add(new DataGridTextColumn { Header = "Preț (RON)", Binding = new Binding("Pret") { StringFormat = "{0} Lei" }, Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
                dg.Columns.Add(new DataGridTextColumn { Header = "Tip Catalog", Binding = new Binding("Tip"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });

                dg.ItemsSource = db.ProduseServicii.ToList();

                StackPanel sp = new StackPanel { Margin = new Thickness(10) };
                sp.Children.Add(CreeazaTitluEcran("💈 Catalog de Produse și Servicii Disponibile"));
                sp.Children.Add(dg);

                Grid panelButoane = new Grid { Margin = new Thickness(0, 20, 0, 0) };
                panelButoane.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                panelButoane.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Button btnModifica = CreeazaButonActiune("⚙️ Adaugă Produs / Serviciu Nou", Colors.Indigo);
                btnModifica.HorizontalAlignment = HorizontalAlignment.Left;
                btnModifica.Click += (s, args) =>
                {
                    Window wDialog = new Window { Title = "Adaugă în Catalog", Width = 320, Height = 320, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)), Foreground = Brushes.White, ResizeMode = ResizeMode.NoResize };
                    StackPanel panel = new StackPanel { Margin = new Thickness(20) };
                    panel.Children.Add(new TextBlock { Text = "Denumire:", Margin = new Thickness(0, 5, 0, 2), FontWeight = FontWeights.Bold });
                    TextBox txtNume = new TextBox { Height = 25, Margin = new Thickness(0, 0, 0, 10), Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), Foreground = Brushes.White, BorderBrush = Brushes.Gray }; panel.Children.Add(txtNume);
                    panel.Children.Add(new TextBlock { Text = "Preț (număr):", Margin = new Thickness(0, 5, 0, 2), FontWeight = FontWeights.Bold });
                    TextBox txtPret = new TextBox { Height = 25, Margin = new Thickness(0, 0, 0, 10), Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), Foreground = Brushes.White, BorderBrush = Brushes.Gray }; panel.Children.Add(txtPret);
                    panel.Children.Add(new TextBlock { Text = "Tip:", Margin = new Thickness(0, 5, 0, 2), FontWeight = FontWeights.Bold });
                    ComboBox cbTip = new ComboBox { Height = 25, Margin = new Thickness(0, 0, 0, 20) }; cbTip.Items.Add("Serviciu"); cbTip.Items.Add("Produs"); cbTip.SelectedIndex = 0; panel.Children.Add(cbTip);

                    Button btnSalva = new Button { Content = "💾 Adaugă în Catalog", Height = 35, Background = Brushes.Indigo, Foreground = Brushes.White, FontWeight = FontWeights.Bold, BorderBrush = Brushes.Transparent };
                    btnSalva.Click += (sDel, argsDel) =>
                    {
                        decimal pretValidat;
                        if (string.IsNullOrEmpty(txtNume.Text) || !decimal.TryParse(txtPret.Text, out pretValidat)) { MessageBox.Show("Introduceți o denumire validă și un preț corect numeric!", "Atenție"); return; }
                        using (var dbCtx = new BazaDeDateContext())
                        {
                            dbCtx.ProduseServicii.Add(new ProdusServiciu { Denumire = txtNume.Text, Pret = pretValidat, Tip = cbTip.SelectedItem.ToString() });
                            dbCtx.SaveChanges();
                        }
                        MessageBox.Show("Adăugat cu succes în catalog!", "Succes"); wDialog.Close(); ActualizeazaTabelProduse();
                    };
                    panel.Children.Add(btnSalva); wDialog.Content = panel; wDialog.ShowDialog();
                };

                Button btnStergeProdus = CreeazaButonActiune("🗑️ Șterge Elementul Selectat", Colors.DarkRed);
                btnStergeProdus.HorizontalAlignment = HorizontalAlignment.Right;
                btnStergeProdus.Click += (s, args) =>
                {
                    var selectat = dg.SelectedItem as ProdusServiciu;
                    if (selectat != null)
                    {
                        var resp = MessageBox.Show($"Sigur doriți să ștergeți '{selectat.Denumire}'?", "Confirmare Ștergere", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (resp == MessageBoxResult.Yes)
                        {
                            using (var dbCtx = new BazaDeDateContext())
                            {
                                var pDeSters = dbCtx.ProduseServicii.Find(selectat.Id);
                                if (pDeSters != null) { dbCtx.ProduseServicii.Remove(pDeSters); dbCtx.SaveChanges(); MessageBox.Show("Șters!"); }
                            }
                            ActualizeazaTabelProduse();
                        }
                    }
                    else { MessageBox.Show("Selectați un rând!"); }
                };

                Grid.SetColumn(btnModifica, 0); panelButoane.Children.Add(btnModifica);
                Grid.SetColumn(btnStergeProdus, 1); panelButoane.Children.Add(btnStergeProdus);
                sp.Children.Add(panelButoane);
                ZonaCentrala.Content = sp;
            }
        }

        // 4. ECRANUL PLATI & INCASARI
        private void BtnPlati_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new BazaDeDateContext())
                {
                    // Luam programarile platite
                    var platiEfectuate = db.Programari.Where(p => p.Status.Contains("Plătită")).ToList();
                    var produseCatalog = db.ProduseServicii.ToList();

                    // Calculam suma totala incasata pe loc prin LINQ
                    decimal totalIncasat = 0;
                    foreach (var p in platiEfectuate)
                    {
                        var serviciu = produseCatalog.FirstOrDefault(s => s.Id == p.IdServiciu);
                        if (serviciu != null) totalIncasat += serviciu.Pret;
                    }

                    StackPanel sp = new StackPanel { Margin = new Thickness(10) };
                    sp.Children.Add(CreeazaTitluEcran("💳 Registru Jurnal de Plăți & Încasări"));

                    DataGrid dg = CreeazaDataGridStilizat();
                    dg.Columns.Add(new DataGridTextColumn { Header = "ID Tranzacție", Binding = new Binding("Id"), Width = new DataGridLength(100) });
                    dg.Columns.Add(new DataGridTextColumn { Header = "ID Client decontat", Binding = new Binding("IdClient"), Width = new DataGridLength(1, DataGridLengthUnitType.Star) });
                    dg.Columns.Add(new DataGridTextColumn { Header = "Data Încasării", Binding = new Binding("DataOra") { StringFormat = "{0:dd.MM.yyyy}" }, Width = new DataGridLength(150) });
                    dg.Columns.Add(new DataGridTextColumn { Header = "Status Plată", Binding = new Binding("Status"), Width = new DataGridLength(150) });
                    dg.ItemsSource = platiEfectuate;
                    sp.Children.Add(dg);

                    Border bTotal = new Border { Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)), CornerRadius = new CornerRadius(5), Margin = new Thickness(0, 20, 0, 0), Padding = new Thickness(15) };
                    TextBlock txtTotal = new TextBlock { Text = $"💰 TOTAL ÎNCASAT ÎN CASĂ: {totalIncasat} RON", FontSize = 16, Foreground = Brushes.LightGreen, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Right };
                    bTotal.Child = txtTotal;
                    sp.Children.Add(bTotal);

                    ZonaCentrala.Content = sp;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // 5. ECRANUL RAPOARTE PROFIT

        private void BtnRapoarte_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new BazaDeDateContext())
                {
                    var toateProgramarile = db.Programari.ToList();
                    var catalog = db.ProduseServicii.ToList();

                    decimal profitRealizat = 0;
                    decimal venituriEstimateViitoare = 0;

                    foreach (var p in toateProgramarile)
                    {
                        var articol = catalog.FirstOrDefault(c => c.Id == p.IdServiciu);
                        if (articol != null)
                        {
                            if (p.Status.Contains("Plătită")) profitRealizat += articol.Pret;
                            else if (p.Status == "Confirmată") venituriEstimateViitoare += articol.Pret;
                        }
                    }

                    int nrClientiUnici = toateProgramarile.Select(p => p.IdClient).Distinct().Count();

                    StackPanel sp = new StackPanel { Margin = new Thickness(10) };
                    sp.Children.Add(CreeazaTitluEcran("📊 Analytics & Rapoarte Profit Barber Shop"));

                    UniformGrid ugCarduri = new UniformGrid { Columns = 3, Margin = new Thickness(0, 10, 0, 0), Height = 100 };

                    ugCarduri.Children.Add(CreeazaCardStatistica("PROFIT NET", $"{profitRealizat} RON", Colors.SeaGreen));
                    ugCarduri.Children.Add(CreeazaCardStatistica("ESTIMAT VIITOR", $"{venituriEstimateViitoare} RON", Colors.RoyalBlue));
                    ugCarduri.Children.Add(CreeazaCardStatistica("CLIENȚI UNICI", $"{nrClientiUnici} Pers.", Colors.DarkGoldenrod));

                    sp.Children.Add(ugCarduri);

                    TextBlock txtAnaliza = new TextBlock
                    {
                        Text = $"📈 Notă Analiză: Salonul înregistrează o activitate stabilă. Rata de conversie a clienților este optimă, iar veniturile estimate în valoare de {venituriEstimateViitoare} RON din programările confirmate indică un flux de numerar pozitiv pentru zilele următoare.",
                        Foreground = Brushes.LightGray,
                        Margin = new Thickness(0, 30, 0, 0),
                        FontSize = 13,
                        TextWrapping = TextWrapping.Wrap,
                        FontStyle = FontStyles.Italic
                    };
                    sp.Children.Add(txtAnaliza);

                    ZonaCentrala.Content = sp;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private Border CreeazaCardStatistica(string titlu, string valoare, Color culoare)
        {
            Border b = new Border { Background = new SolidColorBrush(Color.FromRgb(45, 45, 45)), Margin = new Thickness(5), CornerRadius = new CornerRadius(8), BorderBrush = new SolidColorBrush(culoare), BorderThickness = new Thickness(0, 4, 0, 0) };
            StackPanel sp = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(10) };
            sp.Children.Add(new TextBlock { Text = titlu, FontSize = 11, Foreground = Brushes.Gray, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center });
            sp.Children.Add(new TextBlock { Text = valoare, FontSize = 22, Foreground = Brushes.White, FontWeight = FontWeights.Black, HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 5, 0, 0) });
            b.Child = sp;
            return b;
        }

        private DataGrid CreeazaDataGridStilizat()
        {
            Style stilHeader = new Style(typeof(System.Windows.Controls.Primitives.DataGridColumnHeader));

            stilHeader.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.ForegroundProperty, Brushes.Black));

            stilHeader.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.FontWeightProperty, FontWeights.Bold));

            stilHeader.Setters.Add(new Setter(System.Windows.Controls.Primitives.DataGridColumnHeader.PaddingProperty, new Thickness(5)));

            return new DataGrid
            {
                AutoGenerateColumns = false,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                RowBackground = new SolidColorBrush(Color.FromRgb(40, 40, 40)),
                AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(45, 45, 45)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HorizontalGridLinesBrush = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
                RowHeight = 35,
                FontSize = 13,
                IsReadOnly = true,
                CanUserAddRows = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,

                // Aplicam stilul creat pe tabel
                ColumnHeaderStyle = stilHeader
            };
        }

        private TextBlock CreeazaTitluEcran(string text)
        {
            return new TextBlock { Text = text, FontSize = 18, Foreground = new SolidColorBrush(Color.FromRgb(212, 175, 55)), FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 15) };
        }

        private Button CreeazaButonActiune(string text, Color culoareFundal)
        {
            return new Button { Content = text, Margin = new Thickness(0, 20, 0, 0), Padding = new Thickness(15, 10, 15, 10), Background = new SolidColorBrush(culoareFundal), Foreground = Brushes.White, FontWeight = FontWeights.Bold, BorderBrush = Brushes.Transparent, Height = 40 };
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWin = new MainWindow();
            loginWin.Show();
            this.Close();
        }
    }
}