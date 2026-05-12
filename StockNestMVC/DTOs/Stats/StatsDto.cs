using StockNestMVC.DTOs.Notification;

namespace StockNestMVC.DTOs.Stats;

public class StatsDto
{
        public int TotalGroups { get; set; }

        public int TotalCategories {  get; set; }

        public int TotalItems { get; set; }

        public int UserCreatedItems {  get; set; }

        public int UserUpdatedItems { get; set; }

        public List<ItemsPerCategoryDto> TopCategories { get; set; }
        public List<ItemsPerCategoryDto> ItemsPerGroup { get; set; }

        public IEnumerable<NotificationDto> LatestNotifications { get; set; }

}
