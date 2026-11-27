using System;
using System.Collections.Generic;

namespace KooliProjekt.Application.Features.Public.Dtos
{
    public class MyNotificationDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public bool IsRead { get; set; }
    }

    public class MyReturnDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int OrderNumber { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        public IList<MyReturnLineDto> Lines { get; set; }

        public MyReturnDto()
        {
            Lines = new List<MyReturnLineDto>();
        }
    }

    public class MyReturnLineDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }
    }
}
