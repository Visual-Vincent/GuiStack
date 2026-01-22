/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2025-2026
 * https://github.com/Visual-Vincent/GuiStack
 */

/* eslint-disable @typescript-eslint/no-explicit-any */

export class Template
{
    public render: (() => any) | ((props: any) => any);
    public props: any;

    constructor(snippet: (() => any) | ((props: any) => any), props?: any) {
        this.render = snippet;
        this.props = props;
    }
}
