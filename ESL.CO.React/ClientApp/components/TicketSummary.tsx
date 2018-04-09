import * as React from 'react';

export default class TicketSummary extends React.Component<{ summary: string }> {
    public render() {

        return <div className="TicketText"><h4>{TicketSummary.maxTextSize(this.props.summary)}</h4></div>
    }

    private static maxTextSize(summary: string) {
        let shortSummary = summary;

        if (summary.length > 140) {
            shortSummary = summary.substring(0, 137) + "...";
        }

        return shortSummary;
    }
}