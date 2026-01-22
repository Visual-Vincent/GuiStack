/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2025-2026
 * https://github.com/Visual-Vincent/GuiStack
 */

import type { Bucket, Object } from "$lib/models/s3";
import { ApiClient } from "./apiClient";

export class S3Service
{
    public static async CreateBucketAsync(name: string): Promise<void> {
        await ApiClient.PostAsync(`/api/S3/Buckets/${encodeURIComponent(name)}`);
    }

    public static async DeleteBucketAsync(name: string): Promise<void> {
        await ApiClient.DeleteAsync(`/api/S3/Buckets/${encodeURIComponent(name)}`);
    }

    public static async DeleteObjectAsync(bucketName: string, objectName: string): Promise<void> {
        await ApiClient.DeleteAsync(`/api/S3/Buckets/${encodeURIComponent(bucketName)}/${encodeURIComponent(objectName)}`);
    }

    public static async GetBucketsAsync(): Promise<Bucket[]> {
        return await ApiClient.GetAsync<Bucket[]>("/api/S3/Buckets");
    }

    public static async GetObjectsAsync(bucketName: string): Promise<Object[]> {
        return await ApiClient.GetAsync<Object[]>(`/api/S3/Buckets/${encodeURIComponent(bucketName)}`);
    }

    public static async RenameObjectAsync(bucketName: string, objectName: string, newName: string): Promise<void> {
        await ApiClient.PostAsync(`/api/S3/Buckets/${encodeURIComponent(bucketName)}/rename/${encodeURIComponent(objectName)}/${encodeURIComponent(newName)}`);
    }

    public static GenerateDownloadUrl(bucketName: string, objectName: string): string {
        return ApiClient.GenerateUrl(`/api/S3/Buckets/${encodeURIComponent(bucketName)}/download/${encodeURIComponent(objectName)}`);
    }

    public static async UploadObjectAsync(bucketName: string, file: File, onprogress?: (uploaded: number, total: number) => void): Promise<void> {
        await ApiClient.UploadFileAsync(`/api/S3/Buckets/${encodeURIComponent(bucketName)}/upload`, file, "file", onprogress);
    }
}