using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace QueuServer.Printer
{
    class PrinterManager
    {
        private string type;
        private string number;

        public PrinterManager(string type, string number)
        {
            this.type = type;
            this.number = number;
        }

        public void Print()
        {
            using (var printDocument = new PrintDocument())
            {
                printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(CreateReceipt);
                printDocument.PrintController = new System.Drawing.Printing.StandardPrintController();
                printDocument.Print();

                //var printDialog = new System.Windows.Forms.PrintDialog();
                //printDialog.Document = printDocument;

                //DialogResult result = printDialog.ShowDialog();
                //if (result == DialogResult.OK)
                //{
                //    printDocument.Print();
                //}
            }

        }

        private void CreateReceipt(object sender, PrintPageEventArgs e)
        {
            Font header_font = new Font("Matura MT Script Capitals", 18);       // ok
            Font number1_font = new Font("Arial Black", 32);
            Font number2_font = new Font("Arial", 10);                           // Tipo de numero (ex. Atendimento Prioritário) 
            Font footer_font = new Font("Calibri", 10);
            Font line_font = new Font("Arial Black", 12);
            Font services_font = new Font("Arial", 10);

            float header_fontHeight = header_font.GetHeight();
            float number1_fontHeight = number1_font.GetHeight();
            float number2_fontHeight = number2_font.GetHeight();
            float footer_fontHeight = footer_font.GetHeight();
            float line_fontHeight = line_font.GetHeight();
            float services_fontHeight = services_font.GetHeight();

            Graphics graphic = e.Graphics;

            int offsetY = -85;

            string texto = "Farmácia Filomena";
            send_text(texto, header_font, e, offsetY);
            offsetY = (offsetY + (int)header_fontHeight) - 10;

            texto = "_________________________________";
            send_text(texto, line_font, e, offsetY);
            offsetY = offsetY + (int)line_fontHeight + 25;

            /* ------------------ Numero ----------------------*/

            // Tipo de Atendimento
            texto = type;
            send_text(texto, number2_font, e, offsetY);
            offsetY = (offsetY + (int)number2_fontHeight) + 15;

            // Numero (ex. A001)
            texto = number;
            send_text(texto, number1_font, e, offsetY);
            offsetY = offsetY + (int)number1_fontHeight - 15;

            /* --------------- Farmácia de Serviço ------------- */

            texto = "Farmácia de serviço 24H";
            send_text(texto, line_font, e, offsetY);
            offsetY += (int)services_fontHeight;


            texto = "_________________________________";
            send_text(texto, line_font, e, offsetY);
            offsetY = offsetY + (int)line_fontHeight;

            /* ------------------- Serviços ---------------------- */

            texto = "Consultas Farmacêuticas";
            send_text(texto, services_font, e, offsetY);
            offsetY += (int)services_fontHeight;

            texto = "Podologia - Nutrição - Psicologia";
            send_text(texto, services_font, e, offsetY);
            offsetY += (int)services_fontHeight;

            texto = "Tensão Arterial - Colesterol - Triglicerídeos";
            send_text(texto, services_font, e, offsetY);
            offsetY += (int)services_fontHeight - 10;

            texto = "_________________________________";
            send_text(texto, line_font, e, offsetY);
            offsetY = offsetY + (int)line_fontHeight;

            /* --------------------- footer ---------------------- */

            texto = "www.farmaciafilomena.pt";
            send_text(texto, footer_font, e, offsetY);
            offsetY += (int)services_fontHeight;

            texto = "geral@farmaciafilomena.pt";
            send_text(texto, footer_font, e, offsetY);
            offsetY += (int)services_fontHeight;

            texto = "Tel:227118475";
            send_text(texto, footer_font, e, offsetY);
            offsetY += (int)services_fontHeight + 15;

            texto = ".";        // para dar espaço antes de cortar
            send_text(texto, footer_font, e, offsetY);

            e.HasMorePages = false;
        }

        private void send_text(string text, Font font, System.Drawing.Printing.PrintPageEventArgs a, float yy_offset)
        {
            using (var sf = new StringFormat())
            {
                var txtDataWidth = a.Graphics.MeasureString(text, font).Width;      // tamanho do texto

                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                a.Graphics.DrawString(text, font, new SolidBrush(Color.Black),
                    a.MarginBounds.Left + (a.MarginBounds.Width / 2),
                     a.MarginBounds.Top + yy_offset, sf);
            }
        }

    }
}
