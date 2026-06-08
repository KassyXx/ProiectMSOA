using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Proiect_MSOA___nr_5
{
    public partial class EmployeeWindow : Window
    {
        public EmployeeWindow()
        {
            InitializeComponent();
            IncarcaProgramariAngajat();
        }

        private void IncarcaProgramariAngajat()
        {
            try
            {
                using (var db = new BazaDeDateContext())
                {
                    dgProgramariAngajat.ItemsSource = db.Programari.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la încărcarea activităților: {ex.Message}");
            }
        }

        //TEXT / CHAT
        private void BtnTrimiteMesaj_Click(object sender, RoutedEventArgs e)
        {
            var selectat = dgProgramariAngajat.SelectedItem as Programare;
            if (selectat == null)
            {
                MessageBox.Show("Selectați un client/o programare din tabel pentru a deschide canalul de comunicare!", "Atenție");
                return;
            }

            // Cream o mini fereastra de dialog
            Window wDialog = new Window
            {
                Title = $"Comunicare directă cu Clientul (ID: {selectat.IdClient})",
                Width = 400,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = this,
                Background = new SolidColorBrush(Color.FromRgb(30, 30, 30)),
                Foreground = Brushes.White,
                ResizeMode = ResizeMode.NoResize
            };

            StackPanel panel = new StackPanel { Margin = new Thickness(15) };

            panel.Children.Add(new TextBlock { Text = "Scrieți mesajul către client:", Margin = new Thickness(0, 0, 0, 5), FontWeight = FontWeights.Bold });
            TextBox txtMesaj = new TextBox { Height = 70, TextWrapping = TextWrapping.Wrap, AcceptsReturn = true, Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), Foreground = Brushes.White, BorderBrush = Brushes.Gray };
            panel.Children.Add(txtMesaj);

            Button btnExpediaza = new Button { Content = "✉️ Expediază Mesajul", Height = 35, Margin = new Thickness(0, 15, 0, 0), Background = Brushes.DarkCyan, Foreground = Brushes.White, FontWeight = FontWeights.Bold, BorderBrush = Brushes.Transparent };

            btnExpediaza.Click += (sDel, argsDel) =>
            {
                if (string.IsNullOrEmpty(txtMesaj.Text)) { MessageBox.Show("Nu puteți trimite un mesaj gol!"); return; }

                try
                {
                    using (var db = new BazaDeDateContext())
                    {
                        db.MesajeComunicare.Add(new MesajComunicare
                        {
                            IdClient = selectat.IdClient,
                            Expeditor = "Staff / Barber (Ionuț)",
                            TextMesaj = txtMesaj.Text,
                            DataTrimitere = DateTime.Now
                        });
                        db.SaveChanges();
                    }
                    MessageBox.Show("Mesajul a fost transmis clientului și salvat în jurnalul de chat al acestuia!", "Notificare Trimisă");
                    wDialog.Close();
                }
                catch (Exception ex) { MessageBox.Show($"Eroare la trimitere: {ex.Message}"); }
            };

            panel.Children.Add(btnExpediaza);
            wDialog.Content = panel;
            wDialog.ShowDialog();
        }

        // BUTONUL 1: CONFIRMA TUNSOARE SAU EXPEDIAZA PRODUS
        private void BtnConfirma_Click(object sender, RoutedEventArgs e)
        {
            var selectat = dgProgramariAngajat.SelectedItem as Programare;
            if (selectat == null)
            {
                MessageBox.Show("Vă rugăm să selectați o înregistrare din tabel!", "Atenție");
                return;
            }

            try
            {
                using (var db = new BazaDeDateContext())
                {
                    var pDeActualizat = db.Programari.Find(selectat.Id);
                    if (pDeActualizat != null)
                    {
                        if (pDeActualizat.Status == "În așteptare livrare")
                        {
                            pDeActualizat.Status = "Produs Expediat"; 
                            db.SaveChanges();
                            MessageBox.Show($"Comandă confirmată! Produsul cu ID {selectat.Id} a fost predat către FAN Courier.", "Gestiune Livrare");
                        }
                        else if (pDeActualizat.Status == "În așteptare")
                        {
                            pDeActualizat.Status = "Confirmată"; 
                            db.SaveChanges();
                            MessageBox.Show($"Programarea pentru tunsoare cu ID {selectat.Id} a fost confirmată!", "Gestiune Salon");
                        }
                        else
                        {
                            MessageBox.Show($"Această activitate are deja statusul '{selectat.Status}' și nu mai necesită confirmare.", "Info");
                            return;
                        }
                    }
                }
                IncarcaProgramariAngajat(); // Refresh tabel
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // BUTONUL 2
        private void BtnFinalizeaza_Click(object sender, RoutedEventArgs e)
        {
            var selectat = dgProgramariAngajat.SelectedItem as Programare;
            if (selectat == null)
            {
                MessageBox.Show("Selectați un rând din tabel!", "Atenție");
                return;
            }

            if (selectat.Status == "Finalizată & Plătită")
            {
                MessageBox.Show("Această activitate este deja finalizată și decontată!", "Informație");
                return;
            }

            var rasp = MessageBox.Show($"Doriți să marcați poziția {selectat.Id} ca fiind complet decontată/încasată în casă?", "Decontare", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (rasp == MessageBoxResult.Yes)
            {
                try
                {
                    using (var db = new BazaDeDateContext())
                    {
                        var pDeActualizat = db.Programari.Find(selectat.Id);
                        if (pDeActualizat != null)
                        {
                            pDeActualizat.Status = "Finalizată & Plătită"; 
                            db.SaveChanges();
                            MessageBox.Show("Decontare reușită! Banii au fost adăugați în casă.", "Succes");
                        }
                    }
                    IncarcaProgramariAngajat();
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