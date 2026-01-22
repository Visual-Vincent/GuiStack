/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 * 
 * Copyright © Vincent Bengtsson & Contributors 2025-2026
 * https://github.com/Visual-Vincent/GuiStack
 */

import ModalComponent from "$components/Modal.svelte";
import type { ElementEvent } from "$lib";
import { mount, unmount } from "svelte";

export interface ModalButton
{
    text: string;
    onclick: (modal: ModalComponent, event: ElementEvent<MouseEvent, HTMLButtonElement>) => void;
    disabled?: boolean;
}

export class Modal
{
    public static show(title: string | null, text: string, buttons?: ModalButton[], closeButton: boolean = true, closeOnBackdropClick: boolean = false): void {
        if(!buttons)
        {
            buttons = [{
                text: "OK",
                onclick: (modal) => modal.close(),
                disabled: false
            }];
        }

        const modal = mount(ModalComponent, {
            target: document.body,
            props: {
                title: title,
                text: text,
                shown: true,
                closeButton: closeButton,
                closeOnBackdropClick: closeOnBackdropClick,
                buttons: buttons,
                onclose: () => {
                    unmount(modal);
                }
            }
        });

        modal.show();
    }
}