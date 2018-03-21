import * as React from 'react';
import { Link, NavLink } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';

export class NavMenu extends React.Component<RouteComponentProps<{}>, { version: String }> {

    constructor(props: RouteComponentProps<{}>, context) {
        super(props, context)
        this.state = {
            version: ''
        };

        setInterval(() => {
            fetch('api/version')
                .then(res => res.json() as Promise<string>)
                .then(data => {
                    this.setState((prevState) => {
                        if (prevState.version != '' && prevState.version != data)
                            window.location.reload();
                        return { version: data }
                    });
                })
        }, 5000); // todo: make this pretty
    }

    public render() {
        if (((this.props.location.pathname != '/admin/statistics') &&
            (this.props.location.pathname.substring(0, 20) != '/admin/jiraconnectionstats') && 
            (this.props.location.pathname.substring(0, 6) != '/admin'))
            || (sessionStorage.getItem("JwtToken") === null))
        {
            return null;
        }

        return <div className='main-nav'>
            <div className='navbar navbar-inverse'>
                <div className='navbar-header'>
                    <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
                        <span className='sr-only'>Toggle navigation</span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                    </button>
                    <Link className='navbar-brand' to={'/'}>ESL.CO.React</Link>
                    <span className='version-aside'>{this.state.version}</span>
                </div>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li>
                            <NavLink to={'/'} exact activeClassName='active'>
                                <span className='glyphicon glyphicon-home'></span> KanBan React
                            </NavLink>
                        </li>
                        <li className="dropdown">
                            <a className="dropdown-toggle" data-toggle="dropdown" href="#"> Admin <span className="caret"></span></a>
                            <ul className={["dropdown-menu", "inverse-dropdown"].join(' ')}>
                                <li>
                                    <NavLink to={'/admin'} activeClassName='active'>
                                        <span className='glyphicon glyphicon-th-list'></span> Boardlist
                                    </NavLink>
                                </li>
                                <li>
                                    <NavLink to={'/admin/presentations'} activeClassName='active'>
                                        <span className='glyphicon glyphicon-th-list'></span> PresentationList
                                    </NavLink>
                                </li>
                                <li>
                                    <NavLink to={'/admin/statistics'} activeClassName='active'>
                                        <span className='glyphicon glyphicon-signal'></span> Statistics
                                    </NavLink>
                                </li>
                            </ul>
                        </li>
                    </ul>
                </div>
            </div>
        </div>;
    }
}