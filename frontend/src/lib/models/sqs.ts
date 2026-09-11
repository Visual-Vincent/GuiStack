/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2026
 * https://github.com/Visual-Vincent/GuiStack
 */

export interface Queue
{
    name: string;
    url: string;
}

export interface QueueInfo
{
    approximateNumberOfMessages: number;
    approximateNumberOfMessagesDelayed: number;
    approximateNumberOfMessagesNotVisible: number;
    createdTimestamp: Date;
    delaySeconds: number;
    fifoQueue: boolean;
    lastModifiedTimestamp: Date;
    maximumMessageSize: number;
    messageRetentionPeriod: number;
    queueARN: string;
    queueURL: string;
    receiveMessageWaitTimeSeconds: number;
    visibilityTimeout: number;
}
