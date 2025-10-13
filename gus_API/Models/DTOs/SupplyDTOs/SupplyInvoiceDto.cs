using System;
using System.Collections.Generic;

namespace gus_API.DTOs
{
    public class SupplyDocumentDto
    {
        public int SupplyId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Status { get; set; } = null!;

        // Предприниматель
        public string EntrepreneurFullName { get; set; } = null!;
        public string EntrepreneurShortName { get; set; } = null!;
        public string EntrepreneurInn { get; set; } = null!;
        public string EntrepreneurOgrnip { get; set; } = null!;
        public string EntrepreneurLegalAddress { get; set; } = null!;
        public string EntrepreneurMagazinName { get; set; } = null!;

        // Менеджер
        public string? ManagerFullName { get; set; }
        public string? ManagerEmail { get; set; }
        public string? ManagerRole { get; set; }

        // Товары
        public List<SupplyItemDtoIn> Items { get; set; } = new List<SupplyItemDtoIn>();
    }

    public class SupplyItemDtoIn
    {
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
