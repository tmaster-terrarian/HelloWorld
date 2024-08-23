using System;
using Microsoft.Xna.Framework;

namespace HelloWorld;

public class PlayerInventory
{
    private bool _justCheckin = false;

    private readonly ItemStack[] _items = new ItemStack[10];

    public int Size => _items.Length;

    public ItemStack this[int index]
    {
        get {
            if(index >= 0 && index < Size)
            {
                var item = _items[index]?.Verify();

                if(item is null)
                {
                    _items[index] = null;
                }

                return item;
            }
            return null;
        }
    }

    public bool CanRemove(string id, int index, int count = -1, bool onlyTargetSlot = false)
    {
        _justCheckin = true;
        bool result = TryRemove(id, index, count, onlyTargetSlot);
        _justCheckin = false;
        return result;
    }

    public bool TryRemove(string id, int index, int count = -1, bool onlyTargetSlot = false)
    {
        if(index == -1) index = 0;

        bool ret = false;

        var _s = this[index];
        if(_s is not null && _s.id == id) // target slot is same item kind
        {
            var diffCount = MathHelper.Min(count, _s.Stacks);

            if(count == -1) _s.Stacks = 0;
            else if(!_justCheckin) _s.Stacks -= diffCount;

            if(count != -1) count -= diffCount;

            if(count <= 0 || onlyTargetSlot) // the amount left to remove was exhausted successfully
            {
                return true;
            }
        }
        else
        {
            for(int i = 0; i < _items.Length; i++) // locate slots that have matching data
            {
                var slot = _items[i];
                if(slot is null) continue;
                if(slot.id == id && i != index) // slot is same item kind
                {
                    var diffCount = MathHelper.Min(count, _s.Stacks);

                    if(count == -1) _s.Stacks = 0;
                    else if(!_justCheckin) _s.Stacks -= diffCount;

                    if(count != -1) count -= diffCount;
                    else ret = true;
                }

                if(count == 0) // the amount left to remove was exhausted successfully
                {
                    return true;
                }
            }
        }

        return ret;
    }

    public bool CanInsert(ItemStack item) => CanInsert(item, FindIndexWithType(item));

    public bool CanInsert(ItemStack item, int index)
    {
        _justCheckin = true;
        var ret = TryInsert(ref item, index);
        _justCheckin = false;
        return ret;
    }

    public bool TryInsert(ItemStack item) => TryInsert(ref item, FindIndexWithType(item));
    public bool TryInsert(ItemStack item, int index) => TryInsert(ref item, index);

    public bool TryInsert(ref ItemStack item) => TryInsert(ref item, FindIndexWithType(item));

    public bool TryInsert(ref ItemStack item, int index)
    {
        if(index == -1) index = 0;

        if(index < 0 || index >= Size) throw new IndexOutOfRangeException(nameof(index));

        ArgumentNullException.ThrowIfNull(item?.Verify(), nameof(item));

        if(item.Stacks <= 0) return true;

        // if(!_justCheckin) Console.WriteLine(this._items.MemberwiseToString(false));

        if(this[index] is null) // target slot is empty
        {
            if(!_justCheckin) _items[index] = item;
            return true;
        }

        var count = item.Stacks;

        if(this[index].id == item.id) // target slot is same item kind
        {
            var def = this[index].GetDef();
            var diffCount = MathHelper.Min(count, def.settings.maxStack - this[index].Stacks);
            if(!_justCheckin) this[index].Stacks += diffCount;
            count -= diffCount;
        }
        if(count == 0) // the amount left to distribute was exhausted successfully
        {
            return true;
        }

        for(int i = 0; i < Size; i++) // locate slots that are empty or have matching data
        {
            var slot = this[i];
            if(slot is null) // slot is empty
            {
                if(!_justCheckin) _items[i] = item;
                return true;
            }

            var def = slot.GetDef();

            if(slot.id == item.id) // slot is same item kind
            {
                var diffCount = MathHelper.Min(count, def.settings.maxStack - slot.Stacks);
                if(!_justCheckin) slot.Stacks += diffCount;
                count -= diffCount;
            }
            if(count == 0) // the amount left to distribute was exhausted successfully
            {
                return true;
            }
        }
        if(!_justCheckin) item.Stacks = count;

        return false; // the inventory is FULL
    }

    private int FindIndexWithType(ItemStack itemStack)
    {
        for(int i = 0; i < Size; i++)
        {
            ItemStack slot = this[i];

            if(slot is null) continue;
            if(slot.id != itemStack.id) continue;
            if(slot.Stacks >= slot.GetDef().settings.maxStack) continue;

            return i;
        }
        return -1;
    }
}
