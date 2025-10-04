module.exports = {
  root: true,
  env: {
    node: true,
    es2022: true,
    browser: true,
  },
  extends: [
    "eslint:recommended",
    "@typescript-eslint/recommended",
    "plugin:vue/vue3-essential",
    "plugin:vue/vue3-strongly-recommended",
    "plugin:vue/vue3-recommended",
  ],
  parserOptions: {
    ecmaVersion: 2022,
    parser: "@typescript-eslint/parser",
    sourceType: "module",
  },
  plugins: ["@typescript-eslint", "vue"],
  rules: {
    // TypeScript rules
    "@typescript-eslint/no-unused-vars": ["error", { argsIgnorePattern: "^_" }],
    "@typescript-eslint/no-explicit-any": "warn",
    "@typescript-eslint/explicit-function-return-type": "off",
    "@typescript-eslint/explicit-module-boundary-types": "off",
    "@typescript-eslint/no-non-null-assertion": "warn",

    // Vue rules
    "vue/multi-word-component-names": "off",
    "vue/no-unused-vars": "error",
    "vue/component-definition-name-casing": ["error", "PascalCase"],
    "vue/component-name-in-template-casing": ["error", "PascalCase"],

    // General rules
    "no-console": process.env.NODE_ENV === "production" ? "warn" : "off",
    "no-debugger": process.env.NODE_ENV === "production" ? "warn" : "off",
    "prefer-const": "error",
    "no-var": "error",
    "object-shorthand": "error",
    "prefer-template": "error",
  },
  overrides: [
    {
      files: ["**/__tests__/**/*.[jt]s?(x)", "**/?(*.)+(spec|test).[jt]s?(x)"],
      env: {
        vitest: true,
      },
    },
  ],
};
