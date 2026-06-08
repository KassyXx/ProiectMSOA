using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Proiect_MSOA___nr_5
{
    public partial class ClientWindow : Window
    {
        public class ElementIstoricUman
        {
            public int Id { get; set; }
            public int IdServiciu { get; set; }
            public DateTime DataOra { get; set; }
            public string Status { get; set; }
            public string ObservatiiRapide { get; set; }
        }

        public ClientWindow()
        {
            InitializeComponent();
            IncarcaDateClient();
        }

        private void IncarcaDateClient()
        {
            try
            {
                using (var db = new BazaDeDateContext())
                {
                    var listaArticole = db.ProduseServicii.ToList();
                    dgCatalogClient.ItemsSource = listaArticole;

                    var doarServicii = db.ProduseServicii.Where(p => p.Tip == "Serviciu").ToList();
                    cbServiciiClient.DisplayMemberPath = "Denumire";
                    cbServiciiClient.SelectedValuePath = "Id";
                    cbServiciiClient.ItemsSource = doarServicii;
                    if (doarServicii.Count > 0) cbServiciiClient.SelectedIndex = 0;

                    dpData.SelectedDate = DateTime.Today;
                    cbOra.SelectedIndex = 1;

                    ActualizeazaIstoricClient(db);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea datelor: {ex.Message}");
            }
        }

        // Filtrare LINQ
        private void TxtCautare_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string textCautat = txtCautare.Text.Trim().ToLower();

                using (var db = new BazaDeDateContext())
                {
                    // Daca textul e gol, afisam tot catalogul, altfel filtram dupa denumire
                    if (string.IsNullOrEmpty(textCautat))
                    {
                        dgCatalogClient.ItemsSource = db.ProduseServicii.ToList();
                    }
                    else
                    {
                        var catalogFiltrat = db.ProduseServicii
                            .Where(p => p.Denumire.ToLower().Contains(textCautat))
                            .ToList();

                        dgCatalogClient.ItemsSource = catalogFiltrat;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Eroare la filtrare: {ex.Message}");
            }
        }

        private void ActualizeazaIstoricClient(BazaDeDateContext db)
        {
            var idClientAndrei = db.Utilizatori.Where(u => u.Username == "andrei").Select(u => u.Id).FirstOrDefault();
            if (idClientAndrei == 0) idClientAndrei = 3;

            var rezervariAndrei = db.Programari.Where(p => p.IdClient == idClientAndrei).ToList();
            var catalog = db.ProduseServicii.ToList();

            var istoricUmanizat = rezervariAndrei.Select(p => {
                var articol = catalog.FirstOrDefault(c => c.Id == p.IdServiciu);
                string tipArticol = articol != null ? articol.Tip : "Serviciu";

                string observatie = "";
                if (tipArticol == "Produs")
                {
                    if (p.Status == "În așteptare livrare")
                    {
                        observatie = "⏳ Comanda a fost primită. Așteaptă confirmarea depozitului.";
                    }
                    else if (p.Status == "Produs Expediat" || p.Status == "Finalizată & Plătită")
                    {
                        observatie = "📦 Curier FAN Courier - Livrare în 24-48h!";
                    }
                    else
                    {
                        observatie = "🚚 Pachet recepționat de client.";
                    }
                }
                else
                {
                    if (p.Status == "În așteptare") observatie = "⏳ Așteaptă aprobarea staff-ului.";
                    else if (p.Status == "Confirmată") observatie = "✅ Te așteptăm la salon la ora stabilită!";
                    else observatie = "🧼 Serviciu finalizat cu succes.";
                }

                return new ElementIstoricUman
                {
                    Id = p.Id,
                    IdServiciu = p.IdServiciu,
                    DataOra = p.DataOra,
                    Status = p.Status,
                    ObservatiiRapide = observatie
                };
            }).ToList();

            dgIstoricClient.ItemsSource = istoricUmanizat;
            dgMesajeClient.ItemsSource = db.MesajeComunicare.Where(m => m.IdClient == idClientAndrei).ToList();
        }

        private void BtnRezerva_Click(object sender, RoutedEventArgs e)
        {
            if (cbServiciiClient.SelectedValue == null || dpData.SelectedDate == null || cbOra.SelectedItem == null)
            {
                MessageBox.Show("Vă rugăm să selectați serviciul, data și ora!", "Atenție");
                return;
            }

            try
            {
                DateTime dataSelectata = dpData.SelectedDate.Value;
                if (dataSelectata.DayOfWeek == DayOfWeek.Saturday || dataSelectata.DayOfWeek == DayOfWeek.Sunday)
                {
                    MessageBox.Show("Salonul nostru este închis în weekend! Vă rugăm să alegeți o zi lucrătoare.", "Zile Libere", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int idServiciuAles = (int)cbServiciiClient.SelectedValue;
                string oraSelectata = (cbOra.SelectedItem as System.Windows.Controls.ComboBoxItem).Content.ToString();
                int ore = int.Parse(oraSelectata.Split(':')[0]);
                DateTime dataOraFinala = new DateTime(dataSelectata.Year, dataSelectata.Month, dataSelectata.Day, ore, 0, 0);

                using (var db = new BazaDeDateContext())
                {
                    var idClientAndrei = db.Utilizatori.Where(u => u.Username == "andrei").Select(u => u.Id).FirstOrDefault();
                    if (idClientAndrei == 0) idClientAndrei = 3;

                    db.Programari.Add(new Programare
                    {
                        IdClient = idClientAndrei,
                        IdAngajat = 2,
                        IdServiciu = idServiciuAles,
                        DataOra = dataOraFinala,
                        Status = "În așteptare"
                    });
                    db.SaveChanges();

                    MessageBox.Show("Solicitarea de programare a fost trimisă cu succes!", "Succes");
                    ActualizeazaIstoricClient(db);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCumparaProdus_Click(object sender, RoutedEventArgs e)
        {
            var selectat = dgCatalogClient.SelectedItem as ProdusServiciu;
            if (selectat == null)
            {
                MessageBox.Show("Selectați un produs din tabelul din stânga!", "Atenție");
                return;
            }

            if (selectat.Tip == "Serviciu")
            {
                MessageBox.Show("Pentru servicii folosiți formularul din dreapta!", "Informație");
                return;
            }

            var resultado = MessageBox.Show($"Doriți să comandați '{selectat.Denumire}' la prețul de {selectat.Pret} Lei?", "Confirmare Comandă", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (resultado == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new BazaDeDateContext())
                    {
                        var idClientAndrei = db.Utilizatori.Where(u => u.Username == "andrei").Select(u => u.Id).FirstOrDefault();
                        if (idClientAndrei == 0) idClientAndrei = 3;

                        db.Programari.Add(new Programare
                        {
                            IdClient = idClientAndrei,
                            IdAngajat = 2,
                            IdServiciu = selectat.Id,
                            DataOra = DateTime.Now,
                            Status = "În așteptare livrare"
                        });
                        db.SaveChanges();

                        MessageBox.Show($"Comandă plasată! Statusul a fost setat pe 'În așteptare livrare'.", "Succes");
                        ActualizeazaIstoricClient(db);
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow loginWin = new MainWindow();
            loginWin.Show();
            this.Close();
        }
    }
}