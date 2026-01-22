/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2025-2026
 * https://github.com/Visual-Vincent/GuiStack
 */

import { env } from "$env/dynamic/public";
import { ApiError } from "$lib";

interface ResponseLike
{
    ok: boolean;
    status: number;
    url: string;
    text(): Promise<string>;
}

export class ApiClient
{
    private static apiUrl: string = (() => {
        const url = env.PUBLIC_API_URL;
        
        if(!url || url === "")
            throw new Error("An API URL has not been configured!");

        return url.replace(/\/$/, "");
    })();

    private static leadingSlashRegex = /^\//;

    private static async EnsureSuccessfulAsync(response: ResponseLike) {
        if(response.ok)
            return;

        let message = `Server returned HTTP status code ${response.status}`;
        let responseBody = null;

        try {
            responseBody = await response.text();
            const json = JSON.parse(responseBody);

            if(json.error && typeof json.error === "string")
                message = <string>json.error;

            responseBody = json;
        }
        catch {
            // Intentional no-op
        }

        throw new ApiError(message, response.status, response.url, responseBody);
    }

    private static async MakeRequestAsync<T>(method: string, endpoint: string, body?: BodyInit | null): Promise<T> {
        const response = await fetch(this.GenerateUrl(endpoint), {
            method: method,
            body: body
        });

        await this.EnsureSuccessfulAsync(response);

        const responseBody = await response.text();

        if(!responseBody || responseBody.length == 0)
            return null!;

        return JSON.parse(responseBody);
    }

    private static async MakeXhrRequestAsync<T>(method: string, endpoint: string, body?: XMLHttpRequestBodyInit | null, onprogress?: (uploaded: number, total: number) => void): Promise<T> {
        return new Promise<T>((resolve, reject) => {
            const xhr = new XMLHttpRequest();

            xhr.open(method, this.GenerateUrl(endpoint));

            if(onprogress) {
                xhr.upload.onprogress = (event) => {
                    if(!event.lengthComputable)
                        return;

                    onprogress(event.loaded, event.total);
                };
            }

            xhr.onload = () => {
                this.EnsureSuccessfulAsync({
                    ok: xhr.status >= 200 && xhr.status <= 299,
                    status: xhr.status,
                    url: xhr.responseURL,
                    text: () => new Promise<string>((res) => res(xhr.responseText))
                }).then(() => {
                    if(!xhr.responseText || xhr.responseText.length == 0) {
                        resolve(null!);
                        return;
                    }

                    resolve(JSON.parse(xhr.responseText));
                }).catch((error) => {
                    reject(error);
                });
            };

            xhr.onabort = () => {
                reject(new Error("Request aborted"));
            }

            xhr.onerror = () => {
                reject(new Error("A network error occurred"));
            }

            xhr.send(body);
        });
    }

    public static GenerateUrl(endpoint: string): string {
        return this.apiUrl + "/" + endpoint.replace(this.leadingSlashRegex, "");
    }

    public static async DeleteAsync<T>(endpoint: string, body?: BodyInit | null): Promise<T> {
        return await this.MakeRequestAsync("DELETE", endpoint, body);
    }

    public static async GetAsync<T>(endpoint: string): Promise<T> {
        const response = await fetch(this.GenerateUrl(endpoint), {
            method: "GET",
            cache: "no-store"
        });

        await this.EnsureSuccessfulAsync(response);

        return await response.json();
    }

    public static async PatchAsync<T>(endpoint: string, body?: BodyInit | null): Promise<T> {
        return await this.MakeRequestAsync("PATCH", endpoint, body);
    }

    public static async PostAsync<T>(endpoint: string, body?: BodyInit | null): Promise<T> {
        return await this.MakeRequestAsync("POST", endpoint, body);
    }

    public static async PutAsync<T>(endpoint: string, body?: BodyInit | null): Promise<T> {
        return await this.MakeRequestAsync("PUT", endpoint, body);
    }

    public static async UploadFileAsync(endpoint: string, file: File, fileParameter: string = "file", onprogress?: (uploaded: number, total: number) => void): Promise<void> {
        const formData = new FormData();
        formData.append(fileParameter, file);

        if(onprogress)
            await this.MakeXhrRequestAsync("POST", endpoint, formData, onprogress);
        else
            await this.MakeRequestAsync("POST", endpoint, formData);
    }
}
