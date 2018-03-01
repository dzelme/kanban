import * as React from 'react';

export default class TicketSummary extends React.Component<{ desc: string }> {
    public render() {
        return <div>
            <h5>{this.props.desc}</h5>
        </div>

    }
}