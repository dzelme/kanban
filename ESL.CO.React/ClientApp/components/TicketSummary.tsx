import * as React from 'react';

export default class TicketSummary extends React.Component<{ summary: string }> {
    public render() {

        return <div className="TicketText"><h3>{this.props.summary}</h3></div>
    }
}