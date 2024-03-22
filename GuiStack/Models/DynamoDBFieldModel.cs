/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace GuiStack.Models
{
    public class DynamoDBFieldModel
    {
        public DynamoDBFieldType Type { get; set; }
        public object Value { get; set; }

        private static DynamoDBFieldModel NumberInternal<T>(T number)
            where T : struct
        {
            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.Number,
                Value = number
            };
        }

        private static DynamoDBFieldModel NumberSetInternal<T>(IEnumerable<T> numbers)
            where T : struct
        {
            if(numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.NumberSet,
                Value = numbers.ToArray()
            };
        }

        public static DynamoDBFieldModel Binary(byte[] data)
        {
            if(data == null)
                throw new ArgumentNullException(nameof(data));

            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.Binary,
                Value = Convert.ToBase64String(data)
            };
        }

        public static DynamoDBFieldModel BinarySet(IEnumerable<byte[]> dataSets)
        {
            if(dataSets == null)
                throw new ArgumentNullException(nameof(dataSets));

            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.BinarySet,
                Value = dataSets.Select(data => Convert.ToBase64String(data)).ToArray()
            };
        }

        public static DynamoDBFieldModel Bool(bool value)
        {
            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.Bool,
                Value = value
            };
        }

        public static DynamoDBFieldModel List(IEnumerable<DynamoDBFieldModel> list)
        {
            if(list == null)
                throw new ArgumentNullException(nameof(list));

            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.List,
                Value = list.ToArray()
            };
        }

        public static DynamoDBFieldModel Map(IDictionary<string, DynamoDBFieldModel> map)
        {
            if(map == null)
                throw new ArgumentNullException(nameof(map));

            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.Map,
                Value = map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            };
        }

        public static DynamoDBFieldModel Null()
        {
            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.Null
            };
        }

        public static DynamoDBFieldModel String(string str)
        {
            if(str == null)
                throw new ArgumentNullException(nameof(str));

            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.String,
                Value = str
            };
        }

        public static DynamoDBFieldModel StringSet(IEnumerable<string> strings)
        {
            if(strings == null)
                throw new ArgumentNullException(nameof(strings));

            return new DynamoDBFieldModel() {
                Type = DynamoDBFieldType.StringSet,
                Value = strings.ToArray()
            };
        }

        public static DynamoDBFieldModel Number(sbyte number)   => NumberInternal(number);
        public static DynamoDBFieldModel Number(byte number)    => NumberInternal(number);
        public static DynamoDBFieldModel Number(short number)   => NumberInternal(number);
        public static DynamoDBFieldModel Number(ushort number)  => NumberInternal(number);
        public static DynamoDBFieldModel Number(int number)     => NumberInternal(number);
        public static DynamoDBFieldModel Number(uint number)    => NumberInternal(number);
        public static DynamoDBFieldModel Number(long number)    => NumberInternal(number);
        public static DynamoDBFieldModel Number(ulong number)   => NumberInternal(number);
        public static DynamoDBFieldModel Number(float number)   => NumberInternal(number);
        public static DynamoDBFieldModel Number(double number)  => NumberInternal(number);
        public static DynamoDBFieldModel Number(decimal number) => NumberInternal(number);

        public static DynamoDBFieldModel NumberSet(IEnumerable<sbyte> numbers)   => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<byte> numbers)    => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<short> numbers)   => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<ushort> numbers)  => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<int> numbers)     => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<uint> numbers)    => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<long> numbers)    => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<ulong> numbers)   => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<float> numbers)   => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<double> numbers)  => NumberSetInternal(numbers);
        public static DynamoDBFieldModel NumberSet(IEnumerable<decimal> numbers) => NumberSetInternal(numbers);
    }

    public enum DynamoDBFieldType
    {
        Unknown = 0,
        Binary,
        BinarySet,
        Bool,
        List,
        Map,
        Null,
        Number,
        NumberSet,
        String,
        StringSet
    }
}
