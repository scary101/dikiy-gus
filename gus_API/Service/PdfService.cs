using gus_API.DTOs;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System.IO;
using System.Text;

public class PdfService
{
    static PdfService()
    {
        // Регистрация провайдера кодировок для поддержки windows-1252 и других
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    public byte[] GenerateSupplyPdf(SupplyDocumentDto dto)
    {
        using var ms = new MemoryStream();
        var doc = new Document(PageSize.A4, 25, 25, 40, 40); // Увеличены отступы для официального вида
        PdfWriter.GetInstance(doc, ms);
        doc.Open();

        // Шрифты с поддержкой кириллицы
        BaseFont baseFont = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        var titleFont = new Font(baseFont, 16, Font.BOLD);
        var headerFont = new Font(baseFont, 12, Font.BOLD);
        var regularFont = new Font(baseFont, 10);
        var smallFont = new Font(baseFont, 8);

        // Шапка документа с таблицей кодов справа
        var mainTable = new PdfPTable(2) { WidthPercentage = 100 };
        mainTable.SetWidths(new float[] { 3f, 1.5f }); // Левая часть шире, правая уже

        // Левая часть (название документа)
        var leftCell = new PdfPCell();
        leftCell.Border = Rectangle.NO_BORDER;
        leftCell.AddElement(new Paragraph("НАКЛАДНАЯ ПРИЕМКИ ТОВАРОВ", titleFont) { Alignment = Element.ALIGN_LEFT });
        leftCell.AddElement(new Paragraph("Унифицированная форма № ТОРГ-12", smallFont) { Alignment = Element.ALIGN_LEFT });
        leftCell.AddElement(new Paragraph($"Утверждена постановлением Госкомстата России от 25.12.98 № 132", smallFont) { Alignment = Element.ALIGN_LEFT });
        mainTable.AddCell(leftCell);

        // Правая часть — таблица с кодами
        var rightTable = new PdfPTable(2) { WidthPercentage = 100 };
        rightTable.SetWidths(new float[] { 1f, 1f });
        rightTable.AddCell(new PdfPCell(new Phrase("Форма по ОКУД", headerFont)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 2 });
        rightTable.AddCell(new PdfPCell(new Phrase("0330212", regularFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 2 });
        rightTable.AddCell(new PdfPCell(new Phrase("по ОКПО", headerFont)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 2 });
        rightTable.AddCell(new PdfPCell(new Phrase("66480000", regularFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 2 });
        rightTable.AddCell(new PdfPCell(new Phrase("Номер документа", headerFont)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 2 });
        rightTable.AddCell(new PdfPCell(new Phrase(dto.SupplyId.ToString(), regularFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 2 });
        rightTable.AddCell(new PdfPCell(new Phrase("Дата составления", headerFont)) { HorizontalAlignment = Element.ALIGN_LEFT, Padding = 2 });
        rightTable.AddCell(new PdfPCell(new Phrase(dto.CreatedAt.HasValue ? dto.CreatedAt.Value.ToString("dd.MM.yyyy") : "Не указана", regularFont)) { HorizontalAlignment = Element.ALIGN_RIGHT, Padding = 2 });

        var rightCell = new PdfPCell(rightTable);
        rightCell.Border = Rectangle.BOX;
        mainTable.AddCell(rightCell);

        doc.Add(mainTable);
        doc.Add(new Paragraph(" ")); // Отступ после шапки

        // Информация об отправителе (Поставщик)
        doc.Add(new Paragraph("Поставщик:", headerFont));
        doc.Add(new Paragraph($"{dto.EntrepreneurFullName}, ИНН: {dto.EntrepreneurInn}", regularFont));
        doc.Add(new Paragraph($"Магазин: {dto.EntrepreneurMagazinName}", regularFont));
        doc.Add(new Paragraph($"Юридический адрес: {dto.EntrepreneurLegalAddress}", regularFont));
        doc.Add(new Paragraph(new Chunk(new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 0)))); // Линия
        doc.Add(new Paragraph(" "));

        // Информация о получателе (Маркетплейс)
        doc.Add(new Paragraph("Получатель (Маркетплейс):", headerFont));
        doc.Add(new Paragraph("ООО 'ДИКИЙ ГУСЬ'", regularFont)); // Универсальная заглушка для маркетплейса
        doc.Add(new Paragraph("Юридический адрес: г. Москва, ул. Пушкина, д. 1", regularFont)); // Универсальная заглушка
        doc.Add(new Paragraph(new Chunk(new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 0)))); // Линия
        doc.Add(new Paragraph(" "));

        // Менеджер
        if (!string.IsNullOrEmpty(dto.ManagerFullName))
            doc.Add(new Paragraph($"Ответственный: {dto.ManagerFullName}, Email: {dto.ManagerEmail}", regularFont));

        doc.Add(new Paragraph(" "));

        // Таблица товаров
        if (dto.Items != null && dto.Items.Count > 0)
        {
            var table = new PdfPTable(4) { WidthPercentage = 100 };
            table.SetWidths(new float[] { 1f, 5f, 2f, 2f });

            // Заголовки
            table.AddCell(new PdfPCell(new Phrase("№", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BackgroundColor = new BaseColor(230, 230, 230) });
            table.AddCell(new PdfPCell(new Phrase("Наименование товара", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BackgroundColor = new BaseColor(230, 230, 230) });
            table.AddCell(new PdfPCell(new Phrase("Ед. изм.", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BackgroundColor = new BaseColor(230, 230, 230) });
            table.AddCell(new PdfPCell(new Phrase("Количество", headerFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE, BackgroundColor = new BaseColor(230, 230, 230) });

            // Товары
            int index = 1;
            foreach (var item in dto.Items)
            {
                table.AddCell(new PdfPCell(new Phrase(index.ToString(), regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                table.AddCell(new PdfPCell(new Phrase(item.ProductName.Trim(), regularFont)) { HorizontalAlignment = Element.ALIGN_LEFT, VerticalAlignment = Element.ALIGN_MIDDLE });
                table.AddCell(new PdfPCell(new Phrase("шт.", regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE }); // Универсальная единица
                table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), regularFont)) { HorizontalAlignment = Element.ALIGN_CENTER, VerticalAlignment = Element.ALIGN_MIDDLE });
                index++;
            }

            doc.Add(table);
        }
        else
        {
            doc.Add(new Paragraph("Нет товаров для отображения.", regularFont));
        }

        // Итоговая сумма
        doc.Add(new Paragraph(" "));
        doc.Add(new Paragraph($"Общая стоимость приемки: 2000.00 руб.", regularFont)); // Универсальная заглушка
        doc.Add(new Paragraph(new Chunk(new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 0)))); // Линия
        doc.Add(new Paragraph(" "));

        // Подписи
        var signatureTable = new PdfPTable(2) { WidthPercentage = 100 };
        signatureTable.SetWidths(new float[] { 1f, 1f });
        signatureTable.AddCell(new PdfPCell(new Phrase("Подпись поставщика:", regularFont)) { Border = Rectangle.NO_BORDER });
        signatureTable.AddCell(new PdfPCell(new Phrase("Подпись принявшего:", regularFont)) { Border = Rectangle.NO_BORDER });
        signatureTable.AddCell(new PdfPCell(new Phrase($"ФИО: {dto.EntrepreneurFullName}", regularFont)) { Border = Rectangle.NO_BORDER }); // Используем ФИО ИП
        signatureTable.AddCell(new PdfPCell(new Phrase($"ФИО: {dto.ManagerFullName}", regularFont)) { Border = Rectangle.NO_BORDER }); // Используем ФИО менеджера
        signatureTable.AddCell(new PdfPCell(new Phrase("____________________", regularFont)) { Border = Rectangle.NO_BORDER });
        signatureTable.AddCell(new PdfPCell(new Phrase("____________________", regularFont)) { Border = Rectangle.NO_BORDER });
        doc.Add(signatureTable);

        // Универсальная печать (смещена правее)
        doc.Add(new Paragraph(" "));
        try
        {
            string sealPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Seals", "sup_seal.png");
            if (File.Exists(sealPath))
            {
                var sealTable = new PdfPTable(1) { WidthPercentage = 100 };
                iTextSharp.text.Image sealImage = iTextSharp.text.Image.GetInstance(sealPath);
                sealImage.ScaleToFit(150f, 150f);
                var sealCell = new PdfPCell(sealImage);
                sealCell.HorizontalAlignment = Element.ALIGN_RIGHT; // Выравнивание правее
                sealCell.Border = Rectangle.NO_BORDER;
                sealTable.AddCell(sealCell);
                doc.Add(sealTable);
            }
            else
            {
                doc.Add(new Paragraph("Печать не найдена по пути: " + sealPath, smallFont) { Alignment = Element.ALIGN_CENTER });
            }
        }
        catch (Exception ex)
        {
            doc.Add(new Paragraph($"Ошибка загрузки печати: {ex.Message}", smallFont) { Alignment = Element.ALIGN_CENTER });
        }

        doc.Close();
        return ms.ToArray();
    }
}