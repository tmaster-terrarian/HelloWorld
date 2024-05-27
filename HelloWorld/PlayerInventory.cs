using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace HelloWorld;

public class PlayerInventory
{
    private bool _justCheckin = false;

    private readonly ItemStack[] _items = new ItemStack[10];

    public int Length => _items.Length;

    public ItemStack this[int index]
    {
        get {
            if(index >= 0 && index < _items.Length)
            {
                return _items[index];
            }
            return null;
        }
    }

    public bool CanInsert(ItemStack item, int index)
    {
        _justCheckin = true;
        return TryInsert(item, index);
    }

    public bool TryInsert(ItemStack item, int index)
    {
        if(!(index >= 0 && index < _items.Length)) throw new System.IndexOutOfRangeException(nameof(index));
        if(item == null) throw new System.ArgumentNullException(nameof(item));
        if(item.stacks <= 0) throw new System.IndexOutOfRangeException("stacks cannot be less than or equal to 0");

        if(_items[index] == null) // target slot is empty
        {
            if(!_justCheckin) _items[index] = item;
            return true;
        }

        var count = item.stacks;
        for(int i = 0; i < _items.Length; i++) // locate slots that are empty or have matching data
        {
            var slot = _items[i];
            var def = slot.GetDef();

            if(slot == null) // slot is empty
            {
                if(!_justCheckin) _items[i] = item;
                return true;
            }
            if(slot.id == item.id) // slot is same item kind
            {
                var diffCount = MathHelper.Min(count, def.settings.maxStack - slot.stacks);
                if(!_justCheckin) slot.stacks += diffCount;
                count -= diffCount;
            }
            if(count == 0) // the amount left to distribute was exhausted successfully
            {
                return true;
            }
        }

        if(!_justCheckin) item.stacks = count;

        _justCheckin = false;
        return false; // the inventory is FULL
    }
}
