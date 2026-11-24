(() => {
  const storageKey = "swagger-ui-theme";
  const preference = window.matchMedia("(prefers-color-scheme: dark)");
  let selected = "system";
  try {
    const saved = localStorage.getItem(storageKey);
    if (["system", "light", "dark"].includes(saved)) selected = saved;
  } catch { /* Theme selection also works when storage is unavailable. */ }

  function applyTheme() {
    document.documentElement.dataset.swaggerTheme = selected === "system"
      ? (preference.matches ? "dark" : "light") : selected;
  }

  applyTheme();
  preference.addEventListener("change", applyTheme);

  function mountHeader() {
    const header = document.createElement("header");
    header.className = "api-header";
    const brand = document.createElement("div");
    const title = document.createElement("strong");
    title.textContent = document.title.replace(" Swagger UI", "") + " / API Explorer";
    const caption = document.createElement("span");
    caption.textContent = "Explore endpoints, authorize requests, and inspect responses.";
    brand.append(title, caption);
    const label = document.createElement("label");
    label.textContent = "Theme ";
    const select = document.createElement("select");
    select.setAttribute("aria-label", "Documentation theme");
    for (const [value, text] of [["system", "System"], ["light", "Light"], ["dark", "Dark"]]) {
      select.add(new Option(text, value));
    }
    select.value = selected;
    select.addEventListener("change", () => {
      selected = select.value;
      try { localStorage.setItem(storageKey, selected); } catch { /* Optional persistence. */ }
      applyTheme();
    });
    label.append(select);
    header.append(brand, label);
    document.body.prepend(header);
  }

  if (document.readyState === "loading") document.addEventListener("DOMContentLoaded", mountHeader);
  else mountHeader();
})();
