﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace NonBlocking
{
    internal sealed class NonBlockingDictionaryLong<TValue>
                : NonBlockingDictionary<long, long, TValue>
    {
        internal override bool TryClaimSlotForPut(ref long entryKey, long key, Counter slots)
        {
            return TryClaimSlot(ref entryKey, key, slots);
        }

        internal override bool TryClaimSlotForCopy(ref long entryKey, long key, Counter slots)
        {
            return TryClaimSlot(ref entryKey, key, slots);
        }

        private bool TryClaimSlot(ref long entryKey, long key, Counter slots)
        {
            var entryKeyValue = entryKey;
            //zero keys are claimed via hash
            if (entryKeyValue == 0 & key != 0)
            {
                entryKeyValue = Interlocked.CompareExchange(ref entryKey, key, 0);
                if (entryKeyValue == 0)
                {
                    // claimed a new slot
                    slots.increment();
                    return true;
                }
            }

            if (key == entryKeyValue || keyComparer.Equals(key, entryKey))
            {
                return true;
            }

            return false;
        }

        internal override int hash(long key)
        {
            if (key == 0)
            {
                return ZEROHASH;
            }

            return base.hash(key);
        }

        protected override bool keyEqual(long key, long entryKey)
        {
            return key == entryKey || keyComparer.Equals(key, entryKey);
        }

        protected override NonBlockingDictionary<long, long, TValue> CreateNew()
        {
            return new NonBlockingDictionaryLong<TValue>();
        }
    }

    internal sealed class NonBlockingDictionaryLongNoComparer<TValue>
            : NonBlockingDictionary<long, long, TValue>
    {
        internal override bool TryClaimSlotForPut(ref long entryKey, long key, Counter slots)
        {
            return TryClaimSlot(ref entryKey, key, slots);
        }

        internal override bool TryClaimSlotForCopy(ref long entryKey, long key, Counter slots)
        {
            return TryClaimSlot(ref entryKey, key, slots);
        }

        private bool TryClaimSlot(ref long entryKey, long key, Counter slots)
        {
            var entryKeyValue = entryKey;
            //zero keys are claimed via hash
            if (entryKeyValue == 0 & key != 0)
            {
                entryKeyValue = Interlocked.CompareExchange(ref entryKey, key, 0);
                if (entryKeyValue == 0)
                {
                    // claimed a new slot
                    slots.increment();
                    return true;
                }
            }

            if (key == entryKeyValue)
            {
                return true;
            }

            return false;
        }

        internal override int hash(long key)
        {
            return (key == 0) ?
                ZEROHASH :
                key.GetHashCode() | REGULAR_HASH_BITS;
        }

        protected override bool keyEqual(long key, long entryKey)
        {
            return key == entryKey;
        }

        protected override NonBlockingDictionary<long, long, TValue> CreateNew()
        {
            return new NonBlockingDictionaryLongNoComparer<TValue>();
        }
    }
}
