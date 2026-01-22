/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2025-2026
 * https://github.com/Visual-Vincent/GuiStack
 */

/* eslint-disable @typescript-eslint/no-explicit-any */

export const ProjectName = "GuiStack";

export const TopLevelPages: TopLevelPage[] = [
    { navMenuName: "Dashboard", dashboardName: null,              dashboardIcon: null,                              url: "/",         selectedRegex: new RegExp("^/(?:\\?|$)") },
    { navMenuName: "S3",        dashboardName: "S3 Buckets",      dashboardIcon: "bi bi-bucket",                    url: "/S3",       selectedRegex: new RegExp("^/S3(?:/|\\?|$)") },
    { navMenuName: "SQS",       dashboardName: "SQS Queues",      dashboardIcon: "fa-solid fa-database -rotate-90", url: "/SQS",      selectedRegex: new RegExp("^/SQS(?:/|\\?|$)") },
    { navMenuName: "SNS",       dashboardName: "SNS Topics",      dashboardIcon: "fa-solid fa-square-envelope",     url: "/SNS",      selectedRegex: new RegExp("^/SNS(?:/|\\?|$)") },
    { navMenuName: "DynamoDB",  dashboardName: "DynamoDB Tables", dashboardIcon: "bi bi-table",                     url: "/DynamoDB", selectedRegex: new RegExp("^/DynamoDB(?:/|\\?|$)") },
    { navMenuName: "About",     dashboardName: null,              dashboardIcon: null,                              url: "/About",    selectedRegex: new RegExp("^/About(?:/|\\?|$)") },
];

export type ElementEvent<E extends Event = Event, T extends EventTarget = Element>
    = E & { currentTarget: EventTarget & T };

export interface TopLevelPage
{
    navMenuName: string | null;
    dashboardName: string | null;
    dashboardIcon: string | null;
    url: string;
    selectedRegex: RegExp;
}

export class ApiError extends Error
{
    public statusCode?: number;
    public responseBody?: any;
    public url?: string;

    constructor(message?: string | null, statusCode?: number, url?: string | null, responseBody?: any) {
        super(message ?? undefined);
        Object.setPrototypeOf(this, ApiError.prototype);

        this.statusCode = statusCode;
        this.responseBody = responseBody ?? undefined;
        this.url = url ?? undefined;
    }
}
