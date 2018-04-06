import * as React from 'react';

export default class TicketAssignee extends React.Component<{ assigneeName: string }> {

    public render() {

        return <div className="TicketText">
            <h5>{this.props.assigneeName}</h5>
        </div>
    }
}