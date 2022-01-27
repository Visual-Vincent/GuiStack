// Thanks to: https://khalidabuhakmeh.com/add-svelte-to-aspnet-core-projects

import svelte from 'rollup-plugin-svelte';
import resolve from '@rollup/plugin-node-resolve';

export default {
    input: 'wwwroot/js/site.js',
    output: {
        file: 'wwwroot/js/build/site.bundle.js',
        format: 'cjs', // CommonJS module
        name: 'app',
    },
    plugins: [
        svelte({
            include: 'wwwroot/**/*.svelte',
            emitCss: false,
            compilerOptions: {
                customElement: true
            }
        }),
        // Indicate to third-party plugins that we're building for a web browser
        resolve({ browser: true }),
    ]
};