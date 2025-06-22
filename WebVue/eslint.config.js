import pluginVue from "eslint-plugin-vue";
import eslintConfigPrettier from "eslint-config-prettier";

import globals from "globals";
import eslint from "@eslint/js";

export default [
  eslint.configs.recommended,
  eslintConfigPrettier,
  ...pluginVue.configs["flat/recommended"],
  {
    rules: {
      // override/add rules settings here, such as:
      // 'vue/no-unused-vars': 'error'
    },
    languageOptions: {
      sourceType: "module",
      globals: {
        ...globals.browser,
      },
    },
  },
];
