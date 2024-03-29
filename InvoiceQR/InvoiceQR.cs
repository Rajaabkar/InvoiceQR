﻿using QRCoder;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InvoiceQR
{
    public partial class InvoiceQR : Form
    {
        public InvoiceQR()
        {
            InitializeComponent();
        }

        private void generateQRBtn_Click(object sender, EventArgs e)
        {
            var sallerName = nameTextBox.Text;
            var trn = TRNtextBox.Text;
            var date = dateTimeTextBox.Text;
            var totalWithVat = totalWithVatTextBox.Text;
            var vat = VATTotalTextBox.Text;

            var qrText = encodeQrText(sallerName,trn,date,totalWithVat,vat);
            richTextBox1.Text = qrText;

            pictureBox1.Image = generateQRImage(qrText);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sallerName">saller name </param>
        /// <param name="sallerTRN"> saller TRN</param>
        /// <param name="invoiceDateTime">invoice date time</param>
        /// <param name="totalWithVAT">total with vat </param>
        /// <param name="VATTotal">vat total</param>
        /// <returns></returns>
        public string encodeQrText(string sallerName, string sallerTRN, string invoiceDateTime, string totalWithVAT, string VATTotal)
        {
            //use UTF8 with sallerName to solve arabic issue
            byte[] bytes = Encoding.UTF8.GetBytes(sallerName);
            string L1 = bytes.Length.ToString("X");
            string tag1Hex = BitConverter.ToString(bytes);
            tag1Hex = tag1Hex.Replace("-", "");

            string L2 = sallerTRN.Length.ToString("X");
            string L3 = invoiceDateTime.Length.ToString("X");
            string L4 = totalWithVAT.Length.ToString("X");
            string L5 = VATTotal.Length.ToString("X");
            //length tag must be 2 digit like '0C' 
            string hex = "01" + ((L1.Length == 1) ? ("0" + L1) : L1) + tag1Hex +
                         "02" + ((L2.Length == 1) ? ("0" + L2) : L2) + ToHexString(sallerTRN) +
                         "03" + ((L3.Length == 1) ? ("0" + L3) : L3) + ToHexString(invoiceDateTime) +
                         "04" + ((L4.Length == 1) ? ("0" + L4) : L4) + ToHexString(totalWithVAT) +
                         "05" + ((L5.Length == 1) ? ("0" + L5) : L5) + ToHexString(VATTotal);

            return HexToBase64(hex);
        }

        private  string ToHexString(string str)
        {
            byte[] bytes = Encoding.Default.GetBytes(str);
            string hexString = BitConverter.ToString(bytes);
            hexString = hexString.Replace("-", "");
            return hexString;
        }

        private  string HexToBase64(string strInput)
        {
            try
            {
                var bytes = new byte[strInput.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(strInput.Substring(i * 2, 2), 16);
                }
                return Convert.ToBase64String(bytes);
            }
            catch (Exception)
            {
                return "-1";
            }
        }
















        private Bitmap generateQRImage(string text)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return qrCodeImage;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String initiaIDIR = @"D:\QRFiles";
            var dialog = new SaveFileDialog();
            dialog.InitialDirectory = initiaIDIR;
            if(dialog.ShowDialog()==DialogResult.OK)

            {
                pictureBox1.Image.Save(dialog.FileName);
            }

        }
    }
}
