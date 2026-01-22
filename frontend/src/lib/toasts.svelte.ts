/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2025-2026
 * https://github.com/Visual-Vincent/GuiStack
 */

export interface Toast
{
    id: string;
    title: string;
    text: string | null;
    type: "info" | "success" | "warning" | "error";
    duration: number | null;
}

class ToastsContainer
{
    public all = $state(<Toast[]>[]);

    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    public exception(title: string, error: any, duration?: number | null): void {
        let text: string | null = null;

        if(error)
        {
            if(error instanceof Error)
                text = error.message;
            else if(typeof error === "string")
                text = error;
        }

        this.push("error", title, text, duration);
    }

    public error(title: string, text?: string | null, duration?: number | null): void {
        this.push("error", title, text, duration);
    }

    public info(title: string, text?: string | null, duration?: number | null): void {
        this.push("info", title, text, duration);
    }

    public success(title: string, text?: string | null, duration?: number | null): void {
        this.push("success", title, text, duration);
    }

    public warning(title: string, text?: string | null, duration?: number | null): void {
        this.push("warning", title, text, duration);
    }

    public push(type: "info" | "success" | "warning" | "error", title: string, text?: string | null, duration?: number | null): void {
        this.all ??= [];
        this.all.push({ id: crypto.randomUUID(), title, text: text ?? null, type, duration: duration ?? null });
    }

    public remove(toast: Toast): void {
        this.all = this.all.filter(t => t !== toast);
    }
}

export const Toasts = $state(new ToastsContainer());
