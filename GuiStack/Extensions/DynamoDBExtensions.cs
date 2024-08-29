/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using GuiStack.Models;

namespace GuiStack.Extensions
{
    public static class DynamoDBExtensions
    {
        private static readonly Dictionary<string, string> DynamoDBBillingModeMap = new Dictionary<string, string>() {
            { BillingMode.PAY_PER_REQUEST.Value, "On-demand" },
            { BillingMode.PROVISIONED.Value, "Provisioned" }
        };

        private static readonly Dictionary<string, string> DynamoDBTableClassMap = new Dictionary<string, string>() {
            { TableClass.STANDARD.Value, "Standard" },
            { TableClass.STANDARD_INFREQUENT_ACCESS.Value, "Standard-IA" }
        };

        private static readonly Dictionary<DynamoDBAttributeType, ScalarAttributeType> DynamoDBScalarAttributeMap = new Dictionary<DynamoDBAttributeType, ScalarAttributeType>() {
            { DynamoDBAttributeType.String, ScalarAttributeType.S },
            { DynamoDBAttributeType.Number, ScalarAttributeType.N },
            { DynamoDBAttributeType.Binary, ScalarAttributeType.B }
        };

        private static readonly Dictionary<string, DynamoDBAttributeType> ScalarDynamoDBAttributeMap =
            DynamoDBScalarAttributeMap.ToDictionary(kvp => kvp.Value.Value, kvp => kvp.Key);

        private static readonly Dictionary<DynamoDBFieldType, string> FieldTypeDynamoDBMap = new Dictionary<DynamoDBFieldType, string>() {
            { DynamoDBFieldType.Binary,    "B" },
            { DynamoDBFieldType.BinarySet, "BS" },
            { DynamoDBFieldType.Bool,      "BOOL" },
            { DynamoDBFieldType.List,      "L" },
            { DynamoDBFieldType.Map,       "M" },
            { DynamoDBFieldType.Null,      "NULL" },
            { DynamoDBFieldType.Number,    "N" },
            { DynamoDBFieldType.NumberSet, "NS" },
            { DynamoDBFieldType.String,    "S" },
            { DynamoDBFieldType.StringSet, "SS" },
        };

        private static readonly Dictionary<string, DynamoDBFieldType> DynamoDBFieldTypeMap =
            FieldTypeDynamoDBMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        private static AttributeValue GetDynamoDBAttributeValue(string name, DynamoDBFieldModel field)
        {
            if(field.Value == null || field.Type == DynamoDBFieldType.Null)
                return new AttributeValue() { NULL = true };

            var value = field.Value;
            var attributeValue = new AttributeValue();

            switch(field.Type)
            {
                case DynamoDBFieldType.String:
                    if(value is not string str)
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a string", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    attributeValue.S = str;
                    break;

                case DynamoDBFieldType.StringSet:
                    if(value is not IEnumerable<string> strList)
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a list of strings", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    attributeValue.SS = strList.ToList();
                    break;

                case DynamoDBFieldType.Number:
                    string numValue;

                    // Not validating numbers as strings: Let DynamoDB handle the parsing and validation
                    if(value is string || value is decimal || (value.GetType().IsPrimitive && value is not char))
                        numValue = value.ToString();
                    else
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a number", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    attributeValue.N = numValue;
                    break;

                case DynamoDBFieldType.NumberSet:
                    if(value is not IEnumerable numList)
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a list of numbers", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    attributeValue.NS = new List<string>();

                    int ni = -1;

                    foreach(var item in numList)
                    {
                        if(!(value is string || value is decimal || (value.GetType().IsPrimitive && value is not char)))
                            throw new AmazonDynamoDBException($"Item at index {ni} of field '{name}' was expected to be a number", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                        attributeValue.NS.Add(value.ToString());
                        ni++;
                    }
                    break;

                case DynamoDBFieldType.Binary:
                    if(value is not string binData)
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a Base64-encoded string", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    try
                    {
                        var data = Convert.FromBase64String(binData);
                        attributeValue.B = new MemoryStream(data);
                    }
                    catch(Exception ex)
                    {
                        throw new AmazonDynamoDBException($"Field '{name}' contains invalid Base64-encoded data", ex, ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);
                    }
                    break;

                case DynamoDBFieldType.BinarySet:
                    if(value is not IEnumerable<string> binDataList)
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a list of Base64-encoded strings", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    attributeValue.BS = new List<MemoryStream>();
                    
                    int bi = 0;

                    try
                    {
                        foreach(var strData in binDataList)
                        {
                            var data = Convert.FromBase64String(strData);
                            attributeValue.BS.Add(new MemoryStream(data));
                            bi++;
                        }
                    }
                    catch(Exception ex)
                    {
                        throw new AmazonDynamoDBException($"Item at index {bi} of field '{name}' contains invalid Base64-encoded data", ex, ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);
                    }
                    break;

                case DynamoDBFieldType.Bool:
                    if(value is not bool boolVal)
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a boolean", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    attributeValue.BOOL = boolVal;
                    break;

                case DynamoDBFieldType.List:
                    if(value is not IEnumerable<DynamoDBFieldModel> list)
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a list of values", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    attributeValue.L = new List<AttributeValue>();

                    int listIndex = -1;
                    foreach(var item in list)
                    {
                        listIndex++;

                        if(item == null)
                            continue;

                        var attributeItem = GetDynamoDBAttributeValue($"{name}[{listIndex}]", item);
                        attributeValue.L.Add(attributeItem);
                    }
                    break;

                case DynamoDBFieldType.Map:
                    if(value is not IDictionary<string, DynamoDBFieldModel> map)
                        throw new AmazonDynamoDBException($"Field '{name}' was expected to be a map of values", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                    attributeValue.M = new Dictionary<string, AttributeValue>();

                    foreach(var kvp in map)
                    {
                        if(string.IsNullOrWhiteSpace(kvp.Key) || kvp.Value == null)
                            continue;

                        var attributeItem = GetDynamoDBAttributeValue($"{name}[{kvp.Key}]", kvp.Value);
                        attributeValue.M.Add(kvp.Key, attributeItem);
                    }
                    break;

                default:
                    throw new AmazonDynamoDBException($"Unknown type '{field.Type}' of field '{name}'", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);
            }

            return attributeValue;
        }

        private static DynamoDBFieldModel GetDynamoDBFieldModel(string name, AttributeValue attribute)
        {
            if(attribute == null || attribute.NULL)
                return new DynamoDBFieldModel() { Type = DynamoDBFieldType.Null };

            switch(GetDynamoDBFieldType(attribute))
            {
                case DynamoDBFieldType.Null:
                    return DynamoDBFieldModel.Null();

                case DynamoDBFieldType.Bool:
                    return DynamoDBFieldModel.Bool(attribute.BOOL);

                case DynamoDBFieldType.Binary:
                    return DynamoDBFieldModel.Binary(attribute.B.ToArray());

                case DynamoDBFieldType.BinarySet:
                    return DynamoDBFieldModel.BinarySet(
                        attribute.BS
                            .Where(stream => stream != null)
                            .Select(stream => stream.ToArray())
                    );

                case DynamoDBFieldType.List:
                    return DynamoDBFieldModel.List(
                        attribute.L
                            .Where(item => item != null)
                            .Select((item, index) => GetDynamoDBFieldModel($"{name}[{index}]", item))
                    );

                case DynamoDBFieldType.Map:
                    return DynamoDBFieldModel.Map(
                        attribute.M
                            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && kvp.Value != null)
                            .ToDictionary(kvp => kvp.Key, kvp => GetDynamoDBFieldModel($"{name}[{kvp.Key}]", kvp.Value))
                    );

                case DynamoDBFieldType.Number:
                    return DynamoDBFieldModel.Number(decimal.Parse(attribute.N, CultureInfo.InvariantCulture));

                case DynamoDBFieldType.NumberSet:
                    return DynamoDBFieldModel.NumberSet(
                        attribute.NS
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => decimal.Parse(s, CultureInfo.InvariantCulture))
                    );

                case DynamoDBFieldType.String:
                    return DynamoDBFieldModel.String(attribute.S);

                case DynamoDBFieldType.StringSet:
                    return DynamoDBFieldModel.StringSet(attribute.SS);
            }

            throw new AmazonDynamoDBException($"Unable to determine type of DynamoDB attribute '{name}'", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);
        }

        private static DynamoDBFieldType GetDynamoDBFieldType(AttributeValue attribute)
        {
            if(attribute.NULL)
                return DynamoDBFieldType.Null;
            if(attribute.IsBOOLSet)
                return DynamoDBFieldType.Bool;
            if(attribute.IsLSet)
                return DynamoDBFieldType.List;
            if(attribute.IsMSet)
                return DynamoDBFieldType.Map;
            if(attribute.B != null)
                return DynamoDBFieldType.Binary;
            if(attribute.BS != null && attribute.BS.Count > 0)
                return DynamoDBFieldType.BinarySet;
            if(!string.IsNullOrWhiteSpace(attribute.N))
                return DynamoDBFieldType.Number;
            if(attribute.NS != null && attribute.NS.Count > 0)
                return DynamoDBFieldType.NumberSet;
            if(attribute.S != null)
                return DynamoDBFieldType.String;
            if(attribute.SS != null && attribute.SS.Count > 0)
                return DynamoDBFieldType.StringSet;

            return DynamoDBFieldType.Unknown;
        }

        /// <summary>
        /// Converts the <see cref="BillingMode"/> into a human-readable string.
        /// </summary>
        /// <param name="entity">The <see cref="BillingMode"/> to convert.</param>
        public static string ToHumanReadableString(this BillingMode entity)
        {
            if(entity == null)
                return "(Unknown)";

            return DynamoDBBillingModeMap.GetValueOrDefault(entity?.Value) ?? "(Unknown)";
        }

        /// <summary>
        /// Converts the <see cref="TableClass"/> into a human-readable string.
        /// </summary>
        /// <param name="entity">The <see cref="TableClass"/> to convert.</param>
        public static string ToHumanReadableString(this TableClass entity)
        {
            if(entity == null)
                return "(Unknown)";

            return DynamoDBTableClassMap.GetValueOrDefault(entity?.Value) ?? "(Unknown)";
        }

        /// <summary>
        /// Converts the <see cref="ScalarAttributeType"/> to <see cref="DynamoDBAttributeType"/>.
        /// </summary>
        /// <param name="attributeType">The <see cref="ScalarAttributeType"/> to convert.</param>
        public static DynamoDBAttributeType ToDynamoDBAttributeType(this ScalarAttributeType attributeType)
        {
            return ScalarDynamoDBAttributeMap.GetValueOrDefault(attributeType.Value);
        }

        /// <summary>
        /// Converts the <see cref="DynamoDBAttributeType"/> to <see cref="ScalarAttributeType"/>.
        /// </summary>
        /// <param name="attributeType">The <see cref="DynamoDBAttributeType"/> to convert.</param>
        public static ScalarAttributeType ToScalarAttributeType(this DynamoDBAttributeType attributeType)
        {
            return DynamoDBScalarAttributeMap.GetValueOrDefault(attributeType);
        }

        /// <summary>
        /// Converts the <see cref="DynamoDBFieldType"/> to the respective DynamoDB type string.
        /// </summary>
        /// <param name="fieldType">The <see cref="DynamoDBFieldType"/> to convert.</param>
        public static string ToDynamoDBType(this DynamoDBFieldType fieldType)
        {
            return FieldTypeDynamoDBMap.GetValueOrDefault(fieldType);
        }

        /// <summary>
        /// Converts a DynamoDB type string into a <see cref="DynamoDBFieldType"/>.
        /// </summary>
        /// <param name="dynamoDbType">The type string to convert.</param>
        public static DynamoDBFieldType StringToDynamoDBFieldType(string dynamoDbType)
        {
            return DynamoDBFieldTypeMap.GetValueOrDefault(dynamoDbType);
        }

        /// <summary>
        /// Converts the item data into a DynamoDB item.
        /// </summary>
        /// <param name="itemData">The item data to convert.</param>
        public static DynamoDBItem ToDynamoDBItem(this IDictionary<string, DynamoDBFieldModel> itemData)
        {
            var result = new DynamoDBItem();

            foreach(var field in itemData)
            {
                if(string.IsNullOrWhiteSpace(field.Key))
                    throw new AmazonDynamoDBException($"DynamoDB field name cannot be empty", ErrorType.Sender, "GuiStack_InvalidField", null, HttpStatusCode.BadRequest);

                var name = field.Key;
                var attributeValue = GetDynamoDBAttributeValue(field.Key, field.Value);

                result.Attributes.Add(name, attributeValue);
            }

            return result;
        }

        /// <summary>
        /// Converts the item data into a DynamoDB item model.
        /// </summary>
        /// <param name="itemData">The item data to convert.</param>
        public static DynamoDBItemModel ToDynamoDBItemModel(this IDictionary<string, AttributeValue> itemData)
        {
            var result = new DynamoDBItemModel();

            foreach(var field in itemData)
            {
                if(string.IsNullOrWhiteSpace(field.Key) || field.Value == null)
                    continue;

                result.Add(field.Key, GetDynamoDBFieldModel(field.Key, field.Value));
            }

            return result;
        }
    }
}
