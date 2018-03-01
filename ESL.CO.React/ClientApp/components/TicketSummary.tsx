import * as React from 'react';

export default class TicketSummary extends React.Component<{ desc: string }> {
    public render() {

        return <div>
            <h3>{this.props.desc}</h3>
        </div>

    }
}