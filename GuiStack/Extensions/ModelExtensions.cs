/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2022-2024
 * https://github.com/Visual-Vincent/GuiStack
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GuiStack.Models;

using ScalarAttributeType = Amazon.DynamoDBv2.ScalarAttributeType;

namespace GuiStack.Extensions
{
    public static class ModelExtensions
    {
        private static readonly Dictionary<DynamoDBAttributeType, ScalarAttributeType> DynamoDBScalarAttributeMap = new Dictionary<DynamoDBAttributeType, ScalarAttributeType>() {
            { DynamoDBAttributeType.String, ScalarAttributeType.S },
            { DynamoDBAttributeType.Number, ScalarAttributeType.N },
            { DynamoDBAttributeType.Binary, ScalarAttributeType.B }
        };
        
        private static readonly Dictionary<string, DynamoDBAttributeType> ScalarDynamoDBAttributeMap = new Dictionary<string, DynamoDBAttributeType>() {
            { ScalarAttributeType.S.Value, DynamoDBAttributeType.String },
            { ScalarAttributeType.N.Value, DynamoDBAttributeType.Number },
            { ScalarAttributeType.B.Value, DynamoDBAttributeType.Binary }
        };

        /// <summary>
        /// Converts the <see cref="IEnumerable"/>&lt;<see cref="S3Object"/>&gt; to <see cref="IEnumerable"/>&lt;<see cref="S3ObjectModel"/>&gt;.
        /// </summary>
        /// <param name="objs">The collection to convert.</param>
        /// <param name="bucketName">The name of the bucket that the objects belong to.</param>
        public static IEnumerable<S3ObjectModel> ToObjectModel(this IEnumerable<S3Object> objs, string bucketName)
        {
            return objs.Select(obj => new S3ObjectModel() {
                BucketName = bucketName,
                Object = obj
            });
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
    }
}
