/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2025-2026
 * https://github.com/Visual-Vincent/GuiStack
 */

const fileSizeSuffixes = ["B", "KB", "MB", "GB", "TB", "PB"];

export function FormatFileSize(size: number, decimals: number = 1): string {
    let i;

    for(i = 0; i < fileSizeSuffixes.length - 1; i++) {
        if(size < Math.pow(1024, i+1))
            return (size / Math.pow(1024, i)).toFixed(decimals) + " " + fileSizeSuffixes[i];
    }

    return (size / Math.pow(1024, i)).toFixed(decimals) + " " + fileSizeSuffixes[i];
}
