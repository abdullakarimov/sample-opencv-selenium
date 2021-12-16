Feature: Image
	Visiting TDL Mobile QA page

@imagecomparison
Scenario: Verify image on the Mobile Application Testing page
	Given that I am on the Main page
	Then I hover over the Platforms link
	Then I click the Mobile application testing link
	Then I verify that the tab title contains Mobile application testing
	Then I compare the first image on the page with the base image
	Then I close the browser