/**
 * Prevents Zone.js from patching deprecated Firefox properties 
 * and causing deprecation warnings in the browser console.
 */
(window as any).__Zone_ignore_on_properties = [
  {
    target: window,
    ignoreProperties: ['mozfullscreenchange', 'mozfullscreenerror', 'fullscreenchange', 'fullscreenerror']
  },
  {
    target: document,
    ignoreProperties: ['mozfullscreenchange', 'mozfullscreenerror', 'fullscreenchange', 'fullscreenerror']
  }
];
(window as any).__zone_symbol__UNPATCHED_EVENTS = ['mozfullscreenchange', 'mozfullscreenerror'];
