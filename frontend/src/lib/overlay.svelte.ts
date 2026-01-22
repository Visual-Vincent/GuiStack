/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2026
 * https://github.com/Visual-Vincent/GuiStack
 */

import type { Template } from "./renderables";

export interface LoadingOverlayElementSettings
{
    spinner?: boolean;
    spinnerRight?: boolean;
    visible?: boolean;
}

export interface LoadingOverlaySettings extends LoadingOverlayElementSettings
{
    contents?: Template | string | null;
}

class LoadingOverlayController
{
    public contents?: Template | string | null = $state();
    public settings: LoadingOverlayElementSettings = $state({});

    public hide(): void {
        this.settings.visible = false;
    }

    public show(contents?: Template | string | null, spinner: boolean = true, spinnerRight: boolean = false): void {
        this.contents = contents;
        this.settings.spinner = spinner;
        this.settings.spinnerRight = spinnerRight;
        this.settings.visible = true;
    }
}

export const LoadingOverlay = $state(new LoadingOverlayController());
