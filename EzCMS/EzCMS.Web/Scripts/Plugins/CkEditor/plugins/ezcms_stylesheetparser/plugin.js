/**
 * This is overrided plugin of stylesheetparser
 * This load all the styles from external css file with external format
 * 09-June-2015
 */

(function () {
	// We want to extract only the elements with classes defined in the stylesheets:
	function parseClasses(aRules, skipSelectors, validSelectors) {
		// aRules are just the different rules in the style sheets
		// We want to merge them and them split them by commas, so we end up with only
		// the selectors
		var s = aRules.join(' ');
		// Remove selectors splitting the elements, leave only the class selector (.)
		s = s.replace(/(,|>|\+|~)/g, ' ');
		// Remove attribute selectors: table[border="0"]
		s = s.replace(/\[[^\]]*/g, '');
		// Remove Ids: div#main
		s = s.replace(/#[^\s]*/g, '');
		// Remove pseudo-selectors and pseudo-elements: :hover :nth-child(2n+1) ::before
		s = s.replace(/\:{1,2}[^\s]*/g, '');

		s = s.replace(/\s+/g, ' ');

		var aSelectors = s.split(' '),
			aClasses = [];

		for (var i = 0; i < aSelectors.length; i++) {
			var selector = aSelectors[i];

			if (validSelectors.test(selector) && !skipSelectors.test(selector)) {
				// If we still don't know about this one, add it
				if (CKEDITOR.tools.indexOf(aClasses, selector) == -1)
					aClasses.push(selector);
			}
		}

		return aClasses;
	}

	function loadStylesCSS(theDoc, skipSelectors, validSelectors) {
		var styles = [],
			// It will hold all the rules of the applied stylesheets (except those internal to CKEditor)
			aRules = [],
			i;

		for (i = 0; i < theDoc.styleSheets.length; i++) {
			var sheet = theDoc.styleSheets[i],
				node = sheet.ownerNode || sheet.owningElement;

			// Skip the internal stylesheets
			if (node.getAttribute('data-cke-temp'))
				continue;

			// Exclude stylesheets injected by extensions
			if (sheet.href && sheet.href.substr(0, 9) == 'chrome://')
				continue;

			// Exclude all script except editor.css
			if (sheet.href && sheet.href.indexOf("editor.css") < 0)
				continue;

			// Bulletproof with x-domain content stylesheet.
			try {
				var sheetRules = sheet.cssRules || sheet.rules;
				for (var j = 0; j < sheetRules.length; j++) {
					var cssDefinitions = sheetRules[j].selectorText.split(',');
					for (var k = 0; k < sheetRules.length; k++) {
						if (cssDefinitions[k] != null && cssDefinitions[k].trim() != '')
							aRules.push(cssDefinitions[k]);
					}
				}
			} catch (e) { }
		}

		var aClasses = parseClasses(aRules, skipSelectors, validSelectors);

		// Add each style to our "Styles" collection.
		for (i = 0; i < aClasses.length; i++) {
			var oElement = aClasses[i].split('.'),
				element, sClassName;

			if (!oElement.length) {
				element = '',
				sClassName = oElement;
			} else {
				element = oElement[0].toLowerCase(),
				sClassName = oElement[1];
			}

			styles.push({
				name: sClassName,
				element: !element.length ? 'span' : element,
				attributes: { 'class': sClassName }
			});
		}

		if (styles.length > 0)
			return styles;
		return [];
	}

	// Register a plugin named "ezcms_stylesheetparser".
	CKEDITOR.plugins.add('ezcms_stylesheetparser', {
		init: function (editor) {
			// Stylesheet parser is incompatible with filter (#10136).
			editor.filter.disable();

			var cachedDefinitions;

			editor.once('stylesSet', function (evt) {
				// Cancel event and fire it again when styles are ready.
				evt.cancel();

				// Overwrite editor#getStylesSet asap (contentDom is the first moment
				// when editor.document is ready), but before stylescombo reads styles set (priority 5).
				editor.once('contentDom', function () {
					editor.getStylesSet(function (definitions) {
						// Rules that must be skipped
						var skipSelectors = (/^body\./i),
							// Rules that are valid as whatever
							validSelectors = (/\.\w+/);

						if (definitions == null) definitions = [];

						cachedDefinitions = loadStylesCSS(editor.document.$, skipSelectors, validSelectors).concat(definitions);

						editor.getStylesSet = function (callback) {
							if (cachedDefinitions)
								return callback(cachedDefinitions);
						};

						editor.fire('stylesSet', { styles: cachedDefinitions });
					});
				});
			}, null, null, 1);
		}
	});
})();