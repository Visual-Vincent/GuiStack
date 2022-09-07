using System;

namespace GuiStack.Models
{
    public class DeleteModalModel
    {
        public string ElementId { get; set; }

        public DeleteModalModel(string elementId)
        {
            ElementId = elementId;
        }
    }
}
